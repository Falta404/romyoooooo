using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QR_Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace QR.Application.Services
{
    public class TokenProvideService
    {
        private readonly IConfiguration _configuration;
        public TokenProvideService(IConfiguration configuration) =>
            _configuration = configuration;
        public string CreateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim("userId", user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.Name.ToString()),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_configuration["JWT:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public RefreshToken CreateRefreshToken()
        {
            var refresh = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            return refresh;
        }
    }
}
