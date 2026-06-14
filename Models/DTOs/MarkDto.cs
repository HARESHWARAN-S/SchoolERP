namespace SchoolERP.Models.DTOs
{
    public class MarkEntryDto
    {
        public string Class { get; set; } = string.Empty;
        public string Sec { get; set; } = string.Empty;
        public string ExamName { get; set; } = string.Empty;
        //public string Subject { get; set; } = string.Empty;
        public decimal TotalMarks { get; set; }
        public List<decimal> Marks { get; set; } = new(); // -1 for absent
    }

    public class MarkEntryResponseDto
    {
        public string AdmnNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int RollNo { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public string MarksObtained { get; set; } = string.Empty; // "Absent" or actual mark
        public decimal TotalMarks { get; set; }
    }

    public class StudentMarkResponseDto
    {
        public string ExamName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public string MarksObtained { get; set; } = string.Empty; // "Absent" or actual mark
        public decimal TotalMarks { get; set; }
    }
}