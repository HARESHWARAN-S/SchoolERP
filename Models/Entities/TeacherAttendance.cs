using SchoolERP.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace SchoolERP.Models.Entities
{
    public class TeacherAttendance
    {
        public string TeacherId { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public AttendanceStatus Status { get; set; }

        // Navigation
        public Teacher? Teacher { get; set; }
    }
}