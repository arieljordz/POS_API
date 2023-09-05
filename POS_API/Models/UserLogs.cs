namespace POS_API.Models
{
    public class UserLogs
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime? TimeInDate { get; set; }
        public DateTime? TimeOutDate { get; set; }
    }
}
