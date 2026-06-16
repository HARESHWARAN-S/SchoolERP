namespace SchoolERP.Models.DTOs
{
    public class MarkStudentAttendanceDto
    {
        public List<int> Attendance { get; set; } = new(); 
    }

    public class StudentAttendanceResponseDto
    {
        public string AdmnNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int RollNo { get; set; }
        public DateOnly Date { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class LeaveDetailsResponseDto
    {
        public DateOnly Date { get; set; }
    }
}