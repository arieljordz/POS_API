namespace POS_API.Models
{
    public class TransactionLogs
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Module { get; set; }
        public string? DataFrom { get; set; }
        public string? DataTo { get; set; }
        public string? Action { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
