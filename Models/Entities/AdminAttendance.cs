using SchoolERP.Models.Enums;

namespace SchoolERP.Models.Entities
{
    public class AdminAttendance
    {
        public string AdminId { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public AttendanceStatus Status { get; set; }

        // Navigation
        public Admin? Admin { get; set; }
    }
}