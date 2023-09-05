namespace POS_API.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int SalesId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
        public bool IsPaid { get; set; }
        public bool IsCheckout { get; set; }
        public DateTime? DateOrdered { get; set; }
    }
}
