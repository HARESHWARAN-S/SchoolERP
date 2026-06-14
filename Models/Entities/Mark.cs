using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolERP.Models.Entities
{
    public class Mark
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MarkId { get; set; }
        public string AdmnNo { get; set; } = string.Empty;
        public string ExamName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public string Class { get; set; } = string.Empty;
        public string Sec { get; set; } = string.Empty;
        public decimal MarksObtained { get; set; }
        public decimal TotalMarks { get; set; }

        // Navigation
        public Student? Student { get; set; }
    }
}