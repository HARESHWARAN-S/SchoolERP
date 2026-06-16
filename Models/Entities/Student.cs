using System;
using System.ComponentModel.DataAnnotations;
using SchoolERP.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema; 


namespace SchoolERP.Models.Entities
{
    public class Student
    {
        [Key]
        [ForeignKey(nameof(Login))]
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
        public int TotalDays { get; set; } = 0;
        public int PresentDays { get; set; } = 0;
        public decimal AttendancePercentage { get; set; } = 0.00m;
        public Login Login { get; set; } = null!;
    }
}
