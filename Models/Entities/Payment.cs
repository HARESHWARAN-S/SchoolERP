using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolERP.Models.Entities
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceNo { get; set; }
        public int FeeId { get; set; }
        public string AdmnNo { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }
        public string StripePaymentId { get; set; } = string.Empty;

        // Navigation
        public Fee? Fee { get; set; }
        public Student? Student { get; set; }
    }
}