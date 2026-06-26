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

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            await _loginService.ForgotPasswordAsync(dto);
            return Ok("Reset code sent to your registered email");
        }

        // Reset Password — verifies code and sets new password
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            await _loginService.ResetPasswordAsync(dto);
            return Ok("Password reset successfully. Please login with your new password.");
        }
    }
}