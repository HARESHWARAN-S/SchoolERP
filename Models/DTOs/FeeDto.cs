namespace SchoolERP.Models.DTOs
{
    public class CreateFeeDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Class { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
    }

    public class FeeResponseDto
    {
        public int FeeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string AdmnNo { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class FeeDueResponseDto
    {
        public int FeeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateOnly DueDate { get; set; }
    }


}