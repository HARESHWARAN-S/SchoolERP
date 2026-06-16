namespace SchoolERP.Models.DTOs
{
    public class MarkAdminAttendanceDto
    {
        public int Status { get; set; } 
    }

    public class AdminAttendanceResponseDto
    {
        public string AdminId { get; set; } = string.Empty;
        public string AdminName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalDays { get; set; }
        public int PresentDays { get; set; }
        public decimal AttendancePercentage { get; set; }
    }

    public class AdminLeaveDetailsResponseDto
    {
        public DateOnly Date { get; set; }
    }
}