namespace SchoolERP.Models.DTOs
{
    public class CreateHomeworkDto
    {
        public string Class { get; set; } = string.Empty;
        public string Sec { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class HomeworkResponseDto
    {
        public int HomeworkId { get; set; }
        public string Class { get; set; } = string.Empty;
        public string Sec { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class StudentHomeworkResponseDto
    {
        public DateOnly Date { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}