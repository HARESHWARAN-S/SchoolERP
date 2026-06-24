using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Services.Interfaces;
using SchoolERP.Models.DTOs;
using System.Security.Claims;

namespace SchoolERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Student")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ILoginService _loginService;
        public StudentController(
            IStudentService studentService,
            ILoginService loginService)
        {
            _studentService = studentService;
            _loginService = loginService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.Name)!;
        }

        [HttpGet("MyProfile")]
        public async Task<IActionResult> GetMyDetails()
        {
            string studentAdmnNo = GetCurrentUserId(); // ← from JWT token
            var result = await _studentService.GetMyDetailsAsync(studentAdmnNo);
            return Ok(result);
        }

        [HttpGet("view-notifications")]
        public async Task<IActionResult> GetMyNotifications()
        {
            var result = await _studentService.GetMyNotificationsAsync();
            return Ok(result);
        }

        [HttpGet("MyTimeTable")]
        public async Task<IActionResult> GetMyTimeTable()
        {
            string admnNo = GetCurrentUserId(); // ← from JWT token
            var result = await _studentService.GetMyTimeTableAsync(admnNo);
            return Ok(result);
        }
        
        [HttpGet("homework")]
        public async Task<IActionResult> GetHomework()
        {
            string admnNo = GetCurrentUserId();
            var result = await _studentService.GetHomeworkAsync(admnNo);
            return Ok(result);
        }

        [HttpPost("pay-fee/{feeId}")]
        public async Task<IActionResult> PayFee(int feeId)
        {
            string admnNo = GetCurrentUserId();
            var result = await _studentService.PayFeeAsync(admnNo, feeId);
            return Ok(result);
        }

        [HttpGet("fee-due")]
        public async Task<IActionResult> GetFeeDue()
        {
            string admnNo = GetCurrentUserId();
            var result = await _studentService.GetFeeDueAsync(admnNo);
            return Ok(result);
        }

        [HttpGet("payment-history")]
        public async Task<IActionResult> GetPaymentHistory()
        {
            string admnNo = GetCurrentUserId();
            var result = await _studentService.GetPaymentHistoryAsync(admnNo);
            return Ok(result);
        }

        [HttpGet("leave-details")]
        public async Task<IActionResult> GetLeaveDetails()
        {
            string admnNo = GetCurrentUserId();
            var result = await _studentService.GetLeaveDetailsAsync(admnNo);
            return Ok(result);
        }

        [HttpGet("marks")]
        public async Task<IActionResult> GetMarks()
        {
            string admnNo = GetCurrentUserId();
            var result = await _studentService.GetMarksAsync(admnNo);
            return Ok(result);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangeMyPassword([FromBody] ChangeMyPasswordDto dto)
        {
            string admnNo = GetCurrentUserId();
            await _loginService.ChangeMyPasswordAsync(admnNo, dto.NewPassword);
            return Ok("Password changed successfully");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            string admnNo = GetCurrentUserId();
            string token = Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "")
                .Trim();
            await _loginService.LogoutAsync(admnNo, token);
            return Ok(new
            {
                statusCode = 200,
                message = "Logged out successfully. Please login again."
            });
        }
    }
}