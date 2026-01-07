namespace FiapCloudGames.Games.Api.Data;

using FiapCloudGames.Games.Api.Models;
using Microsoft.EntityFrameworkCore;

public class GamesContext : DbContext
{
    public DbSet<Game> Games { get; set; }

    public GamesContext(DbContextOptions<GamesContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .HasKey(g => g.Id);

        modelBuilder.Entity<Game>()
            .HasIndex(g => g.Title)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}
