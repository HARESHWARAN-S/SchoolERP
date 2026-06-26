using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Models.DTOs;
using SchoolERP.Services.Interfaces;
using System.Security.Claims;

namespace SchoolERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILoginService _loginService;

        public AdminController(IAdminService adminService,
        ILoginService loginService)
        {
            _adminService = adminService;
            _loginService = loginService;
        }

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

        [HttpGet("Get-Individual-teacher-details/{teacherId}")]
        public async Task<IActionResult> GetTeacher(string teacherId)
        {
            var result = await _adminService.GetTeacherAsync(teacherId);
            return Ok(result);
        }

        [HttpGet("Get-All-teachers")]
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
        /*
        [HttpPut("update-student/{admnNo}/{Class}/{Sec}")]
        public async Task<IActionResult> UpdateStudent(string admnNo,string Class,string Sec)
        {
            await _adminService.UpdateStudentAsync(admnNo,Class,Sec);
            return Ok($"Student '{admnNo}' updated successfully");
        }*/

        [HttpGet("Get-Individual-student-details/{admnNo}")]
        public async Task<IActionResult> GetStudent(string admnNo)
        {
            var result = await _adminService.GetStudentAsync(admnNo);
            return Ok(result);
        }

        [HttpGet("List-All-students")]
        public async Task<IActionResult> GetAllStudents()
        {
            var result = await _adminService.GetAllStudentsAsync();
            return Ok(result);
        }

        [HttpPost("send-notification")]
        public async Task<IActionResult> AddNotification([FromBody] CreateNotificationDto dto)
        {
            var result = await _adminService.AddNotificationAsync(dto);
            return Ok(result);
        }

        [HttpGet("view-notifications")]
        public async Task<IActionResult> GetAllNotifications()
        {
            var result = await _adminService.GetAllNotificationsAsync();
            return Ok(result);
        }

        [HttpPost("add-class")]
        public async Task<IActionResult> AddStudentClass([FromBody] CreateStudentClassDto dto)
        {
            var result = await _adminService.AddStudentClassAsync(dto);
            return Ok(result);
        }

        [HttpGet("Get-All-classes")]
        public async Task<IActionResult> GetAllStudentClasses()
        {
            var result = await _adminService.GetAllStudentClassesAsync();
            return Ok(result);
        }

        [HttpPut("assign-rollnumbers")]
        public async Task<IActionResult> AssignRollNumbers(
            [FromQuery] string className,
            [FromQuery] string sec)
        {
            await _adminService.AssignRollNumbersAsync(className, sec);
            return Ok($"Roll numbers assigned successfully for class '{className}-{sec}'");
        }

        [HttpPost("mark-teacher-attendance")]
        public async Task<IActionResult> MarkTeacherAttendance([FromBody] MarkTeacherAttendanceDto dto)
        {
            var result = await _adminService.MarkTeacherAttendanceAsync(dto);
            return Ok(result);
        }

        [HttpPost("add-subject")]
        public async Task<IActionResult> AddSubject([FromBody] CreateSubjectDto dto)
        {
            var result = await _adminService.AddSubjectAsync(dto);
            return Ok(result);
        }

        [HttpGet("Get-All-subject-Teachers")]
        public async Task<IActionResult> GetAllSubjects()
        {
            var result = await _adminService.GetAllSubjectsAsync();
            return Ok(result);
        }

        [HttpGet("Get-subject-Teachers-ByClass/{className}/{sec}")]
        public async Task<IActionResult> GetSubjectsByClass(string className, string sec)
        {
            var result = await _adminService.GetSubjectsByClassAsync(className, sec);
            return Ok(result);
        }

        [HttpPost("add-fee")]
        public async Task<IActionResult> AddFee([FromBody] CreateFeeDto dto)
        {
            var result = await _adminService.AddFeeAsync(dto);
            return Ok(result);
        }

        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            string adminId = User.FindFirstValue(ClaimTypes.Name)!; // ← from JWT token
            var result = await _adminService.GetMyProfileAsync(adminId);
            return Ok(result);
        }

        [HttpPost("mark-my-attendance")]
        public async Task<IActionResult> MarkMyAttendance([FromBody] MarkAdminAttendanceDto dto)
        {
            string adminId = User.FindFirstValue(ClaimTypes.Name)!;
            var result = await _adminService.MarkMyAttendanceAsync(adminId, dto);
            return Ok(result);
        }

        [HttpGet("leave-details")]
        public async Task<IActionResult> GetMyLeaveDetails()
        {
            string adminId = User.FindFirstValue(ClaimTypes.Name)!;
            var result = await _adminService.GetMyLeaveDetailsAsync(adminId);
            return Ok(result);
        }

        [HttpPut("update-class-timetable")]
        public async Task<IActionResult> UpdateClassTimetable([FromBody] UpdateClassTimetableDto dto)
        {
            var result = await _adminService.UpdateClassTimetableAsync(dto);
            return Ok(result);
        }

        [HttpPut("update-teacher-timetable")]
        public async Task<IActionResult> UpdateTeacherTimetable([FromBody] UpdateTeacherTimetableDto dto)
        {
            var result = await _adminService.UpdateTeacherTimetableAsync(dto);
            return Ok(result);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangeMyPassword([FromBody] ChangeMyPasswordDto dto)
        {
            string adminId = User.FindFirstValue(ClaimTypes.Name)!;
            await _loginService.ChangeMyPasswordAsync(adminId, dto.NewPassword);
            return Ok("Password changed successfully");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            string adminId = User.FindFirstValue(ClaimTypes.Name)!;
            string token = Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "")
                .Trim();
            await _loginService.LogoutAsync(adminId, token);
            return Ok(new
            {
                statusCode = 200,
                message = "Logged out successfully. Please login again."
            });
        }

        [HttpPost("promote-class")]
        public async Task<IActionResult> PromoteClass([FromBody] PromoteClassDto dto)
        {
            var result = await _adminService.PromoteClassAsync(dto);
            return Ok(result);
        }
    }
}