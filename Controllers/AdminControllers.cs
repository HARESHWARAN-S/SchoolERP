using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Models.DTOs;
using SchoolERP.Services.Interfaces;

namespace SchoolERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // ── Requires Admin JWT ────────────────────────────────────────────

        [HttpPost("add-admin")]
        public async Task<IActionResult> AddAdmin([FromBody] CreateAdminDto dto)
        {
            var result = await _adminService.AddAdminAsync(dto);
            return Ok(result);
        }

        [HttpPost("add-teacher")]
        public async Task<IActionResult> AddTeacher([FromBody] CreateTeacherDto dto)
        {
            var result = await _adminService.AddTeacherAsync(dto);
            return Ok(result);
        }

        [HttpPut("remove-teacher/{teacherId}")]
        public async Task<IActionResult> RemoveTeacher(string teacherId)
        {
            await _adminService.RemoveTeacherAsync(teacherId);
            return Ok($"Teacher '{teacherId}' removed successfully");
        }

        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetTeacher(string teacherId)
        {
            var result = await _adminService.GetTeacherAsync(teacherId);
            return Ok(result);
        }

        [HttpGet("teachers")]
        public async Task<IActionResult> GetAllTeachers()
        {
            var result = await _adminService.GetAllTeachersAsync();
            return Ok(result);
        }

        [HttpPost("add-student")]
        public async Task<IActionResult> AddStudent([FromBody] CreateStudentDto dto)
        {
            var result = await _adminService.AddStudentAsync(dto);
            return Ok(result);
        }

        [HttpPut("remove-student/{admnNo}")]
        public async Task<IActionResult> RemoveStudent(string admnNo)
        {
            await _adminService.RemoveStudentAsync(admnNo);
            return Ok($"Student '{admnNo}' removed successfully");
        }

        [HttpGet("student/{admnNo}")]
        public async Task<IActionResult> GetStudent(string admnNo)
        {
            var result = await _adminService.GetStudentAsync(admnNo);
            return Ok(result);
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetAllStudents()
        {
            var result = await _adminService.GetAllStudentsAsync();
            return Ok(result);
        }
        // Add Notification
        [HttpPost("send-notification")]
        public async Task<IActionResult> AddNotification([FromBody] CreateNotificationDto dto)
        {
            var result = await _adminService.AddNotificationAsync(dto);
            return Ok(result);
        }

        // View All Notifications
        [HttpGet("view-notifications")]
        public async Task<IActionResult> GetAllNotifications()
        {
            var result = await _adminService.GetAllNotificationsAsync();
            return Ok(result);
        }
        // Add Student Class
        [HttpPost("add-class")]
        public async Task<IActionResult> AddStudentClass([FromBody] CreateStudentClassDto dto)
        {
            var result = await _adminService.AddStudentClassAsync(dto);
            return Ok(result);
        }
        /*
        // Remove Student Class
        [HttpDelete("remove-class")]
        public async Task<IActionResult> RemoveStudentClass(
            [FromQuery] string className,
            [FromQuery] string sec)
        {
            await _adminService.RemoveStudentClassAsync(className, sec);
            return Ok($"Class '{className}-{sec}' removed successfully");
        }*/

        // Get All Classes
        [HttpGet("classes")]
        public async Task<IActionResult> GetAllStudentClasses()
        {
            var result = await _adminService.GetAllStudentClassesAsync();
            return Ok(result);
        }

        // Mark Teacher Attendance
        [HttpPost("mark-teacher-attendance")]
        public async Task<IActionResult> MarkTeacherAttendance([FromBody] MarkTeacherAttendanceDto dto)
        {
            var result = await _adminService.MarkTeacherAttendanceAsync(dto);
            return Ok(result);
        }
    }
}