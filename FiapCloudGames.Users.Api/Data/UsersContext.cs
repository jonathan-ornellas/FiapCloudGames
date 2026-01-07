namespace FiapCloudGames.Users.Api.Data;

using FiapCloudGames.Users.Api.Models;
using Microsoft.EntityFrameworkCore;

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
            .HasIndex(u => u.Email)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}
