using Lease.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Lease.Api.DataAccess
{
    public class LeaseContext : DbContext
    {
        public LeaseContext(DbContextOptions<LeaseContext> options) : base(options) { }

        public DbSet<LeaseModel> Leases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LeaseModel>().ToTable("Leases");
            modelBuilder.Entity<LeaseModel>(builder =>
            {
                builder.Property(lease => lease.Name).IsRequired();
                builder.Property(lease => lease.StartDate).IsRequired();
                builder.Property(lease => lease.EndDate).IsRequired();
                builder.Property(lease => lease.PaymentAmount).HasColumnType("decimal(12, 2)").IsRequired();
                builder.Property(lease => lease.NumberOfPayments).IsRequired();
                builder.Property(lease => lease.InterestRate).HasColumnType("decimal(5,4)").IsRequired();

            });
        }
    }
}
