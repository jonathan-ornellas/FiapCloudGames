using Fiap.Game.Domain.Entities;
using Fiap.Game.Domain.Entities.Base;
using Fiap.Game.Infra.Data.EntityConfig;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Game.Infra.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> opt) : DbContext(opt)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Domain.Entities.Game> Games => Set<Domain.Entities.Game>();
        public DbSet<LibraryItem> Library => Set<LibraryItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            modelBuilder.ApplyBaseEntityConvention();
        }


        public override int SaveChanges()
        {
            ApplyAudit();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAudit();
            return base.SaveChangesAsync(cancellationToken);
        }
        private void ApplyAudit()
        {
            var now = DateTime.Now;

            foreach (var entry in ChangeTracker.Entries<Entity>())
            {
                if (entry.State == EntityState.Added)
                {
                    var idEntry = entry.Property(nameof(Entity.Id));
                    if (idEntry.CurrentValue is Guid id && id == Guid.Empty)
                        idEntry.CurrentValue = Guid.NewGuid();


                    entry.Property(nameof(Entity.CreatedAt)).CurrentValue = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(Entity.UpdatedAt)).CurrentValue = now;
                }
            }
        }

    }
}
