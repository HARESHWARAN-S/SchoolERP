using SchoolERP.Models.DTOs;

namespace SchoolERP.Services.Interfaces
{
    public interface IStudentService
    {
        Task<StudentResponseDto> GetMyDetailsAsync(string admnNo);
        Task<List<NotificationResponseDto>> GetMyNotificationsAsync();
        Task<string> GetMyTimeTableAsync(string admnNo);
        Task<List<StudentHomeworkResponseDto>> GetHomeworkAsync(string admnNo);
        Task<PayFeeResponseDto> PayFeeAsync(string admnNo, int feeId);
        Task<List<PaymentHistoryResponseDto>> GetPaymentHistoryAsync(string admnNo);
        Task<List<FeeDueResponseDto>> GetFeeDueAsync(string admnNo);
        Task<List<LeaveDetailsResponseDto>> GetLeaveDetailsAsync(string admnNo);
        Task<List<StudentMarkResponseDto>> GetMarksAsync(string admnNo);
    }
}