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
    }
}