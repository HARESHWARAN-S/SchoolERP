using SchoolERP.Models.Enums;

namespace SchoolERP.Models.Entities
{
    public class StudentAttendance
    {
        public string Class { get; set; } = string.Empty;
        public string Sec { get; set; } = string.Empty;
        public string AdmnNo { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public AttendanceStatus Status { get; set; }
        
        public Student? Student { get; set; }
    }
}