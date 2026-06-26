namespace SchoolERP.Models.DTOs
{
    public class AddPTMDto
    {
        public string Class { get; set; } = string.Empty;
        public string Sec { get; set; } = string.Empty;
        public int RollNo { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }

    public class PTMResponseDto
    {
        public DateOnly Date { get; set; }
        public string AdmnNo { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }

    public class StudentPTMResponseDto
    {
        public DateOnly Date { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }
}