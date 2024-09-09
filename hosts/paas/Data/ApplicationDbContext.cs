using btm.paas.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace btm.paas.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<PaymentModel> Payments { get; set; }

        public DbSet<PaymentHistoryModel> PaymentHistories { get; set; }

        public DbSet<MethodActionModel> MethodActions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MethodActionModel>()
                .HasData(
                    new MethodActionModel() { Id = 1, Name = "Deposit" },
                    new MethodActionModel { Id = 2, Name = "Withdrawal" }
                    );
        }
    }
}
