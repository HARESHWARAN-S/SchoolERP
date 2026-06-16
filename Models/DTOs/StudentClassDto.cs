namespace SchoolERP.Models.DTOs
{
    public class CreateStudentClassDto
    {
        public string Class { get; set; } = string.Empty;
        public string Sec { get; set; } = string.Empty;
        public string ClassTimetable { get; set; } = string.Empty;
        public string ClassTeacherId { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty; 
        public int ClassStrength { get; set; } 
    }

    public class StudentClassResponseDto
    {
        public string Class { get; set; } = string.Empty;
        public string Sec { get; set; } = string.Empty;
        public string ClassTimetable { get; set; } = string.Empty;
        public string ClassTeacherId { get; set; } = string.Empty;
        public string ClassTeacherName { get; set; } = string.Empty;
        public int ClassStrength { get; set; }
    }
}