using SchoolERP.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolERP.Models.Entities
{
    public class Fee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public FeeStatus Status { get; set; } = FeeStatus.Unpaid;
        public string AdmnNo { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }

        public Student? Student { get; set; }
    }
}