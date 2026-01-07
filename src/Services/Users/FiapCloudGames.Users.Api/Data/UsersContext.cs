using FiapCloudGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Users.Api.Data;

public class UsersContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public UsersContext(DbContextOptions<UsersContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasConversion(
                v => v.Value,
                v => new FiapCloudGames.Domain.ValueObjects.Email(v));

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}
