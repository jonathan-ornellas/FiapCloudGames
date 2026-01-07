using Fiap.Game.Domain.Entities;
using Fiap.Game.Domain.Interface.Repository;
using Fiap.Game.Domain.Record;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Game.Infra.Data.Repository
{
    public class LibraryRepository(AppDbContext db) : Repository<LibraryItem>(db), ILibraryRepository
    {
        public Task<LibraryItem?> FindAsync(Guid userId, Guid gameId, CancellationToken ct = default)
            => _db.Library.SingleOrDefaultAsync(x => x.UserId == userId && x.GameId == gameId, ct);


        public async Task<IReadOnlyList<LibraryView>> ListByUserAsync(Guid userId, CancellationToken ct = default)
            => await (from li in _db.Library
                      join g in _db.Games on li.GameId equals g.Id
                      where li.UserId == userId
                      orderby li.CreatedAt descending
                      select new LibraryView(g.Id, g.Title, li.PricePaid, li.CreatedAt))
                     .AsNoTracking()
                     .ToListAsync(ct);
    }
}
