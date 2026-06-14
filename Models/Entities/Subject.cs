using System.ComponentModel.DataAnnotations;

namespace SchoolERP.Models.Entities
{
    public class Subject
    {
        public string Class { get; set; } = string.Empty;
        public string Sec { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;

        // Navigation
        public StudentClass? StudentClass { get; set; }
        public Teacher? Teacher { get; set; }
    }
}