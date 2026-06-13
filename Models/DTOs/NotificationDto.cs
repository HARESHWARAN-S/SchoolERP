using SchoolERP.Models.Enums;

namespace SchoolERP.Models.DTOs
{
    public class CreateNotificationDto
    {
        public NotificationTarget Target { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class NotificationResponseDto
    {
        public int NotificationId { get; set; }
        public NotificationTarget Target { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}