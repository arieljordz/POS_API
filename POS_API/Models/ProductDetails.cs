namespace POS_API.Models
{
    public class ProductDetails
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
