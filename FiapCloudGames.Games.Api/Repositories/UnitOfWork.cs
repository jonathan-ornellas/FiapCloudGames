using FiapCloudGames.Domain;
using FiapCloudGames.Games.Api.Data;

namespace FiapCloudGames.Games.Api.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GamesContext _context;

        public UnitOfWork(GamesContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }
    }
}
