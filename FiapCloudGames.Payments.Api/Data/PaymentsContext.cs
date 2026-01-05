namespace FiapCloudGames.Payments.Api.Data;

using FiapCloudGames.Payments.Api.Models;
using Microsoft.EntityFrameworkCore;

public class PaymentsContext : DbContext
{
    public DbSet<Payment> Payments { get; set; }

    public PaymentsContext(DbContextOptions<PaymentsContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Payment>()
            .HasIndex(p => new { p.UserId, p.GameId });

        base.OnModelCreating(modelBuilder);
    }
}
