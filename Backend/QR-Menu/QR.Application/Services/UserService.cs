using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using QR.Application.DTOs;
using QR_Domain.Entities;
using QR_Domain.Interfaces;

namespace QR.Application.Services
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<Result<IEnumerable<GetAllUsersDto>>> GetAllUsers()
        {
            var users = await _unitOfWork.UserRepository.GetAllByAsync(u => u.UserName != "superAdmin");

            var mappedResult = users.Select(u => new GetAllUsersDto(u));

            return new Result<IEnumerable<GetAllUsersDto>>(mappedResult, "Success", HttpStatusCode.OK);
        }

        public async Task<Result<ToggleUserActivityDto>> ToggleActivity(int id)
        {
            var user = await _unitOfWork.UserRepository.FindAsync(u => u.Id == id);
            if(user == null || user.UserName == "superAdmin" || user.Id == id)
            {
                return new Result<ToggleUserActivityDto>(null, "User does not exist.", HttpStatusCode.NotFound);
            }

            bool flag = user.IsActive;

            user.IsActive = !user.IsActive;

            await _unitOfWork.SaveChangesAsync();


            var toggleUserActivityDto = new ToggleUserActivityDto
            {
                Id = user.Id,
                UserName = user.UserName,
                IsActivity = user.IsActive
            };

            if (flag)
                return new Result<ToggleUserActivityDto>(toggleUserActivityDto, "User has been blocked successfully.", HttpStatusCode.OK);

            return new Result<ToggleUserActivityDto>(toggleUserActivityDto, "User has been unblocked successfully.", HttpStatusCode.OK);

        }

        public async Task<Result<IEnumerable<SearchByUserNameDto>>> SearchByUserName(string userName)
        {
            var users = await _unitOfWork.UserRepository.GetAllByAsync(u => u.UserName.Contains(userName));

            var mappedResult = users.Select(u => new SearchByUserNameDto(u));

            return new Result<IEnumerable<SearchByUserNameDto>>(mappedResult, "Success", HttpStatusCode.OK);
        }

        public async Task<Result<UserProfileDto>> GetProfile(int UserId)
        {
            User user = await _unitOfWork.UserRepository.FindAsync(u => u.Id == UserId);

            if (user == null)
                return new Result<UserProfileDto>(null, "User not found.", HttpStatusCode.NotFound);

            var profile = new UserProfileDto
            {
                Name = user.Name,
                UserName = user.UserName ?? null,
                PhoneNumber = user.PhoneNumber ?? null
            };

            return new Result<UserProfileDto>(profile, "Success", HttpStatusCode.OK);
        }

        public async Task<Result<UserProfileUpdateDto>> UpdateProfile(UserProfileUpdateDto NewProfile, int UserId)
        {
            User user = await _unitOfWork.UserRepository.FindAsync(u => u.Id == UserId);

            if (user == null)
                return new Result<UserProfileUpdateDto>(null, "User not found.", HttpStatusCode.NotFound);

            if (!string.IsNullOrEmpty(NewProfile.Name))
                user.Name = NewProfile.Name;
            else
                NewProfile.Name = user.Name;

            if (!string.IsNullOrEmpty(NewProfile.PhoneNumber))
                user.PhoneNumber = NewProfile.PhoneNumber;
            else
                NewProfile.PhoneNumber = user.PhoneNumber;

            await _unitOfWork.SaveChangesAsync();

            return new Result<UserProfileUpdateDto>(NewProfile,"Success", HttpStatusCode.OK);
        }

    }
}
