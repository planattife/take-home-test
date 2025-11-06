using Fundo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Infrastructure.Data
{
    public class LoanDbContext : DbContext
    {
        public LoanDbContext(DbContextOptions<LoanDbContext> options) : base(options)
        { }

        public DbSet<Loan> Loans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Loan>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ApplicantName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.CurrentBalance).HasPrecision(18, 2);
                entity.Property(e => e.Status).IsRequired();
            });

            modelBuilder.Entity<Loan>().HasData(
                new Loan
                {
                    Id = 1,
                    ApplicantName = "John Doe",
                    Amount = 10000m,
                    CurrentBalance = 10000m,
                    Status = "active"
                },
                new Loan
                {
                    Id = 2,
                    ApplicantName = "Jane Doe",
                    Amount = 5000m,
                    CurrentBalance = 4500m,
                    Status = "active"
                },
                new Loan
                {
                    Id = 3,
                    ApplicantName = "Michael Scott",
                    Amount = 15000m,
                    CurrentBalance = 0m,
                    Status = "paid"
                }
            );
        }
    }
}
