namespace POS_API.Models
{
    public class ReceiptDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Discount { get; set; }
        public decimal Change { get; set; }
        public decimal VAT { get; set; }
        public DateTime? DateOrdered { get; set; }
        public DateTime? DateInvoiced { get; set; }
    }
}
