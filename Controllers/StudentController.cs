using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Services.Interfaces;
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

        // Gets teacherId from JWT token automatically
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.Name)!;
        }

        // View own profile — no need to pass teacherId
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
    }
}