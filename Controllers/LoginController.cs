using Microsoft.AspNetCore.Mvc;
using SchoolERP.Models.DTOs;
using SchoolERP.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SchoolERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IAdminService _adminService;

        public LoginController(ILoginService loginService, IAdminService adminService)
        {
            _loginService = loginService;
            _adminService = adminService;
        }

        [HttpPost("setup")]
        [AllowAnonymous]
        public async Task<IActionResult> SetupFirstAdmin([FromBody] CreateFirstAdminDto dto)
        {
            var result = await _adminService.SetupFirstAdminAsync(dto);
            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _loginService.LoginAsync(dto);
            return Ok(result);
        }

        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            await _loginService.ChangePasswordAsync(dto);
            return Ok("Password changed successfully");
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            string username = User.FindFirstValue(ClaimTypes.Name)!;

            string token = Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "")
                .Trim();

            await _loginService.LogoutAsync(username, token);
            Response.Headers["Authorization"] = "";
            Response.Headers["WWW-Authenticate"] = "Bearer";

            return Unauthorized(new
            {
                statusCode = 401,
                message = "Logged out successfully. Please login again."
            });
        }
    }
}