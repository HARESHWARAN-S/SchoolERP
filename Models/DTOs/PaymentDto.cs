namespace SchoolERP.Models.DTOs
{
    public class PayFeeResponseDto
    {
        public int InvoiceNo { get; set; }
        public int FeeId { get; set; }
        public string AdmnNo { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }
        public string StripePaymentId { get; set; } = string.Empty;
    }

    public class PaymentHistoryResponseDto
    {
        public int InvoiceNo { get; set; }
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }
        public string StripePaymentId { get; set; } = string.Empty;
    }
}