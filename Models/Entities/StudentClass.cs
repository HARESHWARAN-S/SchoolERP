using System.ComponentModel.DataAnnotations;

namespace SchoolERP.Models.Entities
{
    public class StudentClass
    {
        public string Class { get; set; } = string.Empty;
        public string Sec { get; set; } = string.Empty;
        public string ClassTimetable { get; set; } = string.Empty;
        public string ClassTeacherId { get; set; } = string.Empty;
        public int ClassStrength { get; set; } = 0;

        // Navigation
        public Teacher? ClassTeacher { get; set; }
    }
}