namespace SchoolERP.Models.DTOs
{
    public class MarkTeacherAttendanceDto
    {
        public string TeacherId { get; set; } = string.Empty;
        public int Status { get; set; } // 0 = Absent, 1 = Present
    }

    public class TeacherAttendanceResponseDto
    {
        public string TeacherId { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalDays { get; set; }
        public int PresentDays { get; set; }
        public decimal AttendancePercentage { get; set; }
    }
}