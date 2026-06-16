using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Services.Interfaces;
using System.Security.Claims;
using SchoolERP.Models.DTOs;

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

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.Name)!;
        }

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
        
        [HttpGet("view-leaveDetails")]
        public async Task<IActionResult> GetMyLeaveDetails()
        {
            string teacherId = GetCurrentUserId();
            var result = await _teacherService.GetMyLeaveDetailsAsync(teacherId);
            return Ok(result);
        }

        [HttpPost("add-homework")]
        public async Task<IActionResult> AddHomework([FromBody] CreateHomeworkDto dto)
        {
            string teacherId = GetCurrentUserId();
            var result = await _teacherService.AddHomeworkAsync(teacherId, dto);
            return Ok(result);
        }

        [HttpPost("mark-attendance")]
        public async Task<IActionResult> MarkStudentAttendance([FromBody] MarkStudentAttendanceDto dto)
        {
            string teacherId = GetCurrentUserId();
            var result = await _teacherService.MarkStudentAttendanceAsync(teacherId, dto);
            return Ok(result);
        }

        [HttpPost("add-marks")]
        public async Task<IActionResult> AddMarks([FromBody] MarkEntryDto dto)
        {
            string teacherId = GetCurrentUserId();
            var result = await _teacherService.AddMarksAsync(teacherId, dto);
            return Ok(result);
        }
    }
}