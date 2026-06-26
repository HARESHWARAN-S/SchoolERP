using System.ComponentModel.DataAnnotations;

namespace SchoolERP.Models.Entities
{
    public class PTM
    {
        public DateOnly Date { get; set; }
        public string AdmnNo { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public string? Remarks { get; set; }

        // Navigation
        public Student? Student { get; set; }
        public Teacher? Teacher { get; set; }
        public StudentClass? StudentClass { get; set; }
    }
}