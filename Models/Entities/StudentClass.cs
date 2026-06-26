using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolERP.Models.Entities
{
    public class StudentClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClassId { get; set; }     
        public string Class { get; set; } = string.Empty;
        public string Sec { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string ClassTimetable { get; set; } = string.Empty;
        public string ClassTeacherId { get; set; } = string.Empty;
        public int ClassStrength { get; set; } = 0;

        public Teacher? ClassTeacher { get; set; }
    }
}