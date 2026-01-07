using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Games.Api.Data;
using FiapCloudGames.Games.Business;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Games.Api.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly GamesContext _context;

        public GameRepository(GamesContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Game game, CancellationToken ct = default)
        {
            _context.Games.Add(game);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Game>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Games.ToListAsync(ct);
        }
    }
}
