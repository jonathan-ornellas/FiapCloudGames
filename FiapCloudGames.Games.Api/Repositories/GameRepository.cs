using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Games.Api.Data;
using FiapCloudGames.Games.Business;

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
            await _context.Games.AddAsync(new Models.Game { Title = game.Title, Description = game.Description, Genre = game.Genre, Price = game.Price.Value, Rating = (int)game.Rating }, ct);
        }
    }
}
