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
        public int ClassId { get; set; } 
        public decimal MarksObtained { get; set; }
        public decimal TotalMarks { get; set; }

        public Student? Student { get; set; }
        public StudentClass? StudentClass { get; set; }
    }
}