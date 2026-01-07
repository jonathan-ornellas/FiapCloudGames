using Fiap.Game.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Game.Infra.Data.Repository
{
    public class GameRepository(AppDbContext db) : Repository<Domain.Entities.Game>(db), IGameRepository
    {
        public Task<Domain.Entities.Game?> GetAsync(Guid id, CancellationToken ct = default)
              => _db.Games.FirstOrDefaultAsync(g => g.Id == id, ct);

        public IQueryable<Domain.Entities.Game> QueryActive() => _db.Games.AsNoTracking().Where(g => g.IsActive);
    }
}
