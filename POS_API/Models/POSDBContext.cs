using Microsoft.EntityFrameworkCore;

namespace POS_API.Models
{
    public class POSDBContext : DbContext
    {
        public POSDBContext(DbContextOptions<POSDBContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetails> ProductDetails { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Sales> Sales { get; set; }
        public DbSet<UserLogs> UserLogs { get; set; }
        public DbSet<TransactionLogs> TransactionLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=JORDZ-PC\\SQLEXPRESS;Initial Catalog=POS_DB;User ID=sa;Password=P@ssw0rd;Trusted_Connection=True;MultipleActiveResultSets=True;Encrypt=False");
        }
    }
}
