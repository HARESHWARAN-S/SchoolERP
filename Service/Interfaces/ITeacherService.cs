using SchoolERP.Models.DTOs;

namespace SchoolERP.Services.Interfaces
{
    public interface ITeacherService
    {
        Task<TeacherResponseDto> GetMyDetailsAsync(string teacherId);
        Task<List<NotificationResponseDto>> GetMyNotificationsAsync();
        Task<string> GetMyTimeTableAsync(string teacherId);
        Task<List<TeacherLeaveDetailsDto>> GetMyLeaveDetailsAsync(string teacherId);
        Task<HomeworkResponseDto> AddHomeworkAsync(string teacherId, CreateHomeworkDto dto);
        Task<List<StudentAttendanceResponseDto>> MarkStudentAttendanceAsync(string teacherId, MarkStudentAttendanceDto dto);
        Task<List<MarkEntryResponseDto>> AddMarksAsync(string teacherId, MarkEntryDto dto);
    }
}