using System;
using System.ComponentModel.DataAnnotations;
using SchoolERP.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema; 
namespace SchoolERP.Models.Entities
{
    public class Admin
    {
        [Key]
        [ForeignKey(nameof(Login))]        
        public string AdminId { get; set; } = string.Empty;
        public string AdminName { get; set; } = string.Empty;
        public DateOnly DOB { get; set; }
        public BloodGroup BloodGrp { get; set; } // Direct Enum Type
        public Gender Gender { get; set; } // Direct Enum Type
        public string ContactNo { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public int TotalDays { get; set; } = 0;
        public int PresentDays { get; set; } = 0;
        public decimal AttendancePercentage { get; set; } = 0.00m;
        public Login Login { get; set; } = null!;
    }
}
