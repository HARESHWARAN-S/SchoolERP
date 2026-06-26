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

    public class UpdateClassTimetableDto
    {
        public string Class { get; set; } = string.Empty;
        public string Sec { get; set; } = string.Empty;
        public string ClassTimetable { get; set; } = string.Empty;
    }
        public class PromoteClassDto
    {
        public string Class { get; set; } = string.Empty;
        public string Sec { get; set; } = string.Empty;
        // index 0 = rollNo 1, values: "A","B",.. = new sec, "0" = fail, "-1" = leave
        public List<string> Promotions { get; set; } = new();
    }

    public class PromoteClassResponseDto
    {
        public string PreviousClass { get; set; } = string.Empty;
        public string PreviousSec { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public int Promoted { get; set; }
        public int Failed { get; set; }
        public int Left { get; set; }
        public List<PromotionDetailDto> Details { get; set; } = new();
    }

    public class PromotionDetailDto
    {
        public string AdmnNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int OldRollNo { get; set; }
        public string OldClass { get; set; } = string.Empty;
        public string OldSec { get; set; } = string.Empty;
        public string NewClass { get; set; } = string.Empty;
        public string NewSec { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // "Promoted", "Failed", "Left"
    }
}