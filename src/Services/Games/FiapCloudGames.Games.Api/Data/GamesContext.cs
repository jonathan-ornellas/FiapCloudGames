using FiapCloudGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Games.Api.Data;

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
            .Property(g => g.Price)
            .HasConversion(
                v => v.Value,
                v => new FiapCloudGames.Domain.ValueObjects.Money(v));

        base.OnModelCreating(modelBuilder);
    }
}
