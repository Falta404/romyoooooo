using QR_Domain.Entities;
using QR_Domain.Interfaces;
using QR.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using QR_Domain.Enums;

namespace QR.Application.Services
{
    public class AuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly TokenProvideService _tokenProvider;
        private readonly IUnitOfWork _unitOfWork;
        public AuthService(UserManager<User> userManager, TokenProvideService tokenProvider, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _tokenProvider = tokenProvider;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<string>> Register(RegisterRequest registerRequest)
        {
            var existingUser = await _userManager.FindByNameAsync(registerRequest.UserName);

            if (existingUser != null)
                return new Result<string>("User already exists", HttpStatusCode.BadRequest);

            if (registerRequest.Password != registerRequest.ConfirmPassword)
                return new Result<string>("Passwords do not match", HttpStatusCode.BadRequest);
            var role = await _unitOfWork.RoleRepository.FindAsync(r => r.Name == Roles.Admin.ToString());

            if (role == null)
                throw new Exception("Role not found");

            var user = new User(registerRequest.UserName, registerRequest.Name, role.Id);

            user.PhoneNumber = registerRequest.PhoneNumber;
            var createUserResult = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!createUserResult.Succeeded)
                return new Result<string>("User creation failed: " +
                    string.Join(", ", createUserResult.Errors.Select(e => e.Description)), HttpStatusCode.BadRequest);

            return new Result<string>("Registration successful", HttpStatusCode.OK);
        }

        public async Task<Result<TokenResponse>> Login(LoginRequest loginDTO)
        {
            var user = await _unitOfWork.UserRepository.FindAsync(u => u.UserName ==loginDTO.UserName, ["Role", "RefreshToken"]);

            if (user == null)
                return new Result<TokenResponse>("Invalid User", HttpStatusCode.NotFound);
            if (!await _userManager.CheckPasswordAsync(user, loginDTO.Password))
                return new Result<TokenResponse>("Invalid Password", HttpStatusCode.Unauthorized);
            if(!user.IsActive)
                return new Result<TokenResponse>("Blocked user.", HttpStatusCode.Unauthorized);

            var refreshToken = _tokenProvider.CreateRefreshToken();

            if (user.RefreshToken != null)
                await _unitOfWork.RefreshTokenRepository.DeleteAsync(user.RefreshToken);
            user.RefreshToken = refreshToken;
            await _unitOfWork.SaveChangesAsync();

            return new Result<TokenResponse>(new TokenResponse(_tokenProvider.CreateAccessToken(user), refreshToken.Token), "Success", HttpStatusCode.OK);
        }

        public async Task<Result<TokenResponse>> GetNewAccessToken(string refresh)
        {
            var user = await _unitOfWork.UserRepository.FindAsync(u => u.RefreshToken.Token == refresh, ["RefreshToken", "Role"]);

            if (user == null || user.RefreshToken == null || refresh != user.RefreshToken.Token || user.RefreshToken.Expires < DateTime.UtcNow || user.IsActive == false)
                return  new Result<TokenResponse>("Invalid Refresh Token", HttpStatusCode.Unauthorized);

            var newrefreshToken = _tokenProvider.CreateRefreshToken();
            if (user.RefreshToken != null)
                await _unitOfWork.RefreshTokenRepository.DeleteAsync(user.RefreshToken);
            user.RefreshToken = newrefreshToken;
            await _unitOfWork.SaveChangesAsync();

            return new Result<TokenResponse>(new TokenResponse(_tokenProvider.CreateAccessToken(user), newrefreshToken.Token), "Success", HttpStatusCode.OK);
        }

        public async Task<Result<string>> Logout(string username)
        {
            var user = await _unitOfWork.UserRepository.FindAsync(u => u.UserName == username, ["RefreshToken"]);

            if (user == null)
                return new Result<string>("Invalid UserName", HttpStatusCode.NotFound);

            if (user.RefreshToken is not null)
                await _unitOfWork.RefreshTokenRepository.DeleteAsync(user.RefreshToken);

            await _unitOfWork.SaveChangesAsync();

            return new Result<string>(null, "Successfully logged out", HttpStatusCode.OK);
        }

        public async Task<User> CreateDefaultUser(string menuName)
        {
            var role = await _unitOfWork.RoleRepository.FindAsync(r => r.Name == Roles.Admin.ToString());

            if (role == null)
                throw new Exception("Role not found");

            var username = $"UserName_{menuName}";
            var password = $"P@ssw0rd_default";
            var user = new User(username, "default", role.Id);

            await _userManager.CreateAsync(user, password);
            return user;
        }

        public async Task<Result<string>> UpdatePassword(UpdatePasswordRequestDto request, int UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId.ToString());

            if (user == null)
                return new Result<string>("User not found", HttpStatusCode.NotFound);

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

            if (!changePasswordResult.Succeeded)
                return new Result<string>("Password update failed: " +
                    string.Join(", ", changePasswordResult.Errors.Select(e => e.Description)), HttpStatusCode.BadRequest);

            return new Result<string>("Password updated successfully", HttpStatusCode.OK);
        }

        public async Task<Result<string>> ResetPasswordAsync(string username)
        {
            var user = await _unitOfWork.UserRepository.FindAsync(u => u.UserName == username);
            if (user == null)
                return new Result<string>("User not found", HttpStatusCode.NotFound);

            var newPassword = GenerateRandomPassword();
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            // will be refactor by phone number
            var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (!resetResult.Succeeded)
                return new Result<string>("Password reset failed: " +
                    string.Join(", ", resetResult.Errors.Select(e => e.Description)), HttpStatusCode.BadRequest);

            return new Result<string>(newPassword, "Success", HttpStatusCode.OK);
        }

        private string GenerateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 10) + "Aa1!";
        }

    }
}


