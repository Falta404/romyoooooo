using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QR.Application.DTOs;
using QR.Application.Helpers;
using QR.Application.Services;
using QR_Domain.Entities;
using QR_Domain.Enums;


namespace QR.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result = await _authService.Login(request);

            return StatusCode((int)result.StatusCode, result);
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result = await _authService.Register(request);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("refresh")]
        public async Task<IActionResult> Refresh(string refreshToken)
        {
            var result = await _authService.GetNewAccessToken(refreshToken);

            return StatusCode((int)result.StatusCode, result);
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _authService.Logout(username);

            return StatusCode((int)result.StatusCode, result);
        }

        [Route("change-password")]
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequestDto request)
        {
            var claims = User.GetClams();
            var res = await _authService.UpdatePassword(request, claims.UserId);

            return StatusCode((int)res.StatusCode, res);
        }

        [Route("reset-password/{username}")]
        [HttpPut]
        [Authorize(Roles = RoleConstants.SuperAdmin)]
        public async Task<IActionResult> ResetPassword(string username)
        {
            var res = await _authService.ResetPasswordAsync(username);

            return StatusCode((int)res.StatusCode, res);

        }
    }
}
