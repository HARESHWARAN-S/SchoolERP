using SchoolERP.Models.Enums;

namespace SchoolERP.Models.DTOs
{
    // First Admin Creation
    public class CreateFirstAdminDto
    {
        public string AdminName { get; set; } = string.Empty;
        public DateOnly DOB { get; set; }
        public Gender Gender { get; set; }
        public BloodGroup BloodGrp { get; set; }
        public string ContactNo { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // New Admin Added By Existing Admin
    public class CreateAdminDto
    {
        public string AdminName { get; set; } = string.Empty;
        public DateOnly DOB { get; set; }
        public Gender Gender { get; set; }
        public BloodGroup BloodGrp { get; set; }
        public string ContactNo { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UpdateAdminDto
    {
        public string AdminName { get; set; } = string.Empty;
        public DateOnly DOB { get; set; }
        public Gender Gender { get; set; }
        public BloodGroup BloodGrp { get; set; }
        public string ContactNo { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
    }

    public class AdminResponseDto
    {
        public string AdminId { get; set; } = string.Empty;
        public string AdminName { get; set; } = string.Empty;
        public DateOnly DOB { get; set; }
        public Gender Gender { get; set; }
        public BloodGroup BloodGrp { get; set; }
        public string ContactNo { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public int TotalDays { get; set; }
        public int PresentDays { get; set; }
        public decimal AttendancePercentage { get; set; }
    }

    public class AdminListDto
    {
        public string AdminId { get; set; } = string.Empty;
        public string AdminName { get; set; } = string.Empty;
        public string ContactNo { get; set; } = string.Empty;
    }

    public class AdminProfileResponseDto
    {
        public string AdminId { get; set; } = string.Empty;
        public string AdminName { get; set; } = string.Empty;
        public DateOnly DOB { get; set; }
        public Gender Gender { get; set; }
        public BloodGroup BloodGrp { get; set; }
        public string ContactNo { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
    }
}