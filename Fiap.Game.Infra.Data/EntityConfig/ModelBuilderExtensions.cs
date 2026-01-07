using Fiap.Game.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Game.Infra.Data.EntityConfig
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyBaseEntityConvention(this ModelBuilder modelBuilder)
        {
            var baseType = typeof(Entity);

            foreach (var et in modelBuilder.Model.GetEntityTypes()
                         .Where(t => baseType.IsAssignableFrom(t.ClrType)))
            {
                var b = modelBuilder.Entity(et.ClrType);
                b.Property(nameof(Entity.Id)).ValueGeneratedNever(); 
                b.Property(nameof(Entity.CreatedAt)).IsRequired();
                b.Property(nameof(Entity.UpdatedAt));
            }
        }
    }
}
