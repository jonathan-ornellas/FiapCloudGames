using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.Game.Infra.Data.EntityConfig
{
    public class GameConfig : IEntityTypeConfiguration<Domain.Entities.Game>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Game> b)
        {
            b.ToTable("Games");
            b.HasKey(x => x.Id);

            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.Price).HasPrecision(18, 2);
            b.Property(x => x.IsActive).HasDefaultValue(true);

            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.UpdatedAt);
        }
    }
}
