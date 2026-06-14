namespace SchoolERP.Models.DTOs
{
    public class CreateSubjectDto
    {
        public string Class { get; set; } = string.Empty;
        public string Sec { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
    }

    public class SubjectResponseDto
    {
        public string Class { get; set; } = string.Empty;
        public string Sec { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
    }
}