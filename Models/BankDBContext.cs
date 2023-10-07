using Microsoft.EntityFrameworkCore;

namespace BankAPI.Models
{
    public class BankDBContext : DbContext
    {
        public BankDBContext() { }

        public BankDBContext(DbContextOptions<BankDBContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
