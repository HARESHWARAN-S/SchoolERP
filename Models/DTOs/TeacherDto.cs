using SchoolERP.Models.Enums;

namespace SchoolERP.Models.DTOs
{
    // Admin Creates Teacher
    public class CreateTeacherDto
    {
        public string Name { get; set; } = string.Empty;
        public DateOnly DOB { get; set; }
        public Gender Gender { get; set; }
        public BloodGroup BloodGrp { get; set; }
        public string ContactNo { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string TimeTableUrl { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UpdateTeacherDto
    {
        public string Name { get; set; } = string.Empty;
        public DateOnly DOB { get; set; }
        public Gender Gender { get; set; }
        public BloodGroup BloodGrp { get; set; }
        public string ContactNo { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string TimeTableUrl { get; set; } = string.Empty;
    }

    public class TeacherResponseDto
    {
        public string TeacherId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateOnly DOB { get; set; }
        public Gender Gender { get; set; }
        public BloodGroup BloodGrp { get; set; }
        public string ContactNo { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string TimeTableUrl { get; set; } = string.Empty;
        public int TotalDays { get; set; }
        public int PresentDays { get; set; }
        public decimal AttendancePercentage { get; set; }
    }

    public class TeacherListDto
    {
        public string TeacherId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
    }
}