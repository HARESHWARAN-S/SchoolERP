using SchoolERP.Models.DTOs;

namespace SchoolERP.Services.Interfaces
{
    public interface IStudentService
    {
        Task<StudentResponseDto> GetMyDetailsAsync(string admnNo);
        Task<List<NotificationResponseDto>> GetMyNotificationsAsync();
        Task<string> GetMyTimeTableAsync(string admnNo);
        Task<List<StudentHomeworkResponseDto>> GetHomeworkAsync(string admnNo);
    }
}