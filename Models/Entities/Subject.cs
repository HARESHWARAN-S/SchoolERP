using System.ComponentModel.DataAnnotations;

namespace SchoolERP.Models.Entities
{
    public class Subject
    {
        public int ClassId { get; set; } 
        public string SubjectName { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;

        public StudentClass? StudentClass { get; set; }
        public Teacher? Teacher { get; set; }
    }
}