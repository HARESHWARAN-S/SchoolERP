using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Services.Interfaces;
using System.Security.Claims;

namespace SchoolERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Teacher")]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;
        private readonly ILoginService _loginService;

        public TeacherController(
            ITeacherService teacherService,
            ILoginService loginService)
        {
            _teacherService = teacherService;
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
            string teacherId = GetCurrentUserId(); // ← from JWT token
            var result = await _teacherService.GetMyDetailsAsync(teacherId);
            return Ok(result);
        }

        [HttpGet("MyTimeTable")]
        public async Task<IActionResult> GetMyTimeTable()
        {
            string teacherId = GetCurrentUserId(); // ← from JWT token
            var result = await _teacherService.GetMyTimeTableAsync(teacherId);
            return Ok(result);
        }

        [HttpGet("view-notifications")]
        public async Task<IActionResult> GetMyNotifications()
        {
            var result = await _teacherService.GetMyNotificationsAsync();
            return Ok(result);
        }
        
    }
}