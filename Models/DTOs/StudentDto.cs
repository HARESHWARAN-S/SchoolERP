using SchoolERP.Models.Enums;

namespace SchoolERP.Models.DTOs
{
    public class CreateStudentDto
    {
        public int RollNo { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string Sec { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public DateOnly DOB { get; set; }
        public BloodGroup BloodGrp { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string MotherName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UpdateStudentDto
    {
        public int RollNo { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string Sec { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public DateOnly DOB { get; set; }
        public BloodGroup BloodGrp { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string MotherName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
    }

    public class StudentResponseDto
    {
        public string AdmnNo { get; set; } = string.Empty;
        public int RollNo { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string Sec { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public DateOnly DOB { get; set; }
        public BloodGroup BloodGrp { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string MotherName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
        public int TotalDays { get; set; }
        public int PresentDays { get; set; }
        public decimal AttendancePercentage { get; set; }
    }

    public class StudentListDto
    {
        public string AdmnNo { get; set; } = string.Empty;
        public int RollNo { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string Sec { get; set; } = string.Empty;
    }
}