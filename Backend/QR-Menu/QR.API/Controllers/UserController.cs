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
    
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = RoleConstants.SuperAdmin)]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var res = await _userService.GetAllUsers();
            return StatusCode((int)res.StatusCode, res);
        }

        [Authorize(Roles = RoleConstants.SuperAdmin)]
        [HttpPut("toggle-activity/{id}")]
        public async Task<IActionResult> ToggleActivity(int id)
        {
            var res = await _userService.ToggleActivity(id);
            return StatusCode((int)res.StatusCode, res);
        }



        [Authorize(Roles = RoleConstants.SuperAdmin)]
        [Route("{UserName}")]
        [HttpGet]
        public async Task<IActionResult> Search(string UserName)
        {
            var res = await _userService.SearchByUserName(UserName);

            return StatusCode((int)res.StatusCode, res);
        }

        [Authorize]
        [Route("get-profile")]
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var claims = User.GetClams();
            var res = await _userService.GetProfile(claims.UserId);

            return StatusCode((int)res.StatusCode, res);
        }

        [Authorize]
        [Route("update-profile")]
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileUpdateDto NewProfile)
        {
            var claims = User.GetClams();
            var res = await _userService.UpdateProfile(NewProfile, claims.UserId);

            return StatusCode((int)res.StatusCode, res);
        }

        


    }
}
