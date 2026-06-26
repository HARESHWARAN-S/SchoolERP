using SchoolERP.Models.Enums;

namespace SchoolERP.Models.Entities
{
    public class StudentAttendance
    {
        public string AdmnNo { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public int ClassId { get; set; } 
        public AttendanceStatus Status { get; set; }
        
        public Student? Student { get; set; }
        public StudentClass? StudentClass { get; set; }
    }
}