namespace POS_API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public ProductDetails? Details { get; set; }
    }
}
