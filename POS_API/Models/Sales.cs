namespace POS_API.Models
{
    public class Sales
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Discount { get; set; }
        public decimal Change { get; set; }
        public DateTime? DateInvoiced { get; set; }
    }
}
