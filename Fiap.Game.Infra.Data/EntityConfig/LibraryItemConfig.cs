using Fiap.Game.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.Game.Infra.Data.EntityConfig
{
    public class LibraryItemConfig : IEntityTypeConfiguration<LibraryItem>
    {
        public void Configure(EntityTypeBuilder<LibraryItem> b)
        {
            b.ToTable("Library");
            b.HasKey(x => new { x.UserId, x.GameId });
            b.HasIndex(x => x.UserId);

            b.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne<Domain.Entities.Game>().WithMany().HasForeignKey(x => x.GameId).OnDelete(DeleteBehavior.Cascade);

            b.Property(x => x.UpdatedAt);
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.PricePaid).HasPrecision(18, 2);
        }
    }
}
