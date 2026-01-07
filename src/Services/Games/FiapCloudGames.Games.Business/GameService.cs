using FiapCloudGames.Domain;
using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Games.Business
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _games;
        private readonly IUnitOfWork _uow;

        public GameService(IGameRepository games, IUnitOfWork uow)
        {
            _games = games;
            _uow = uow;
        }

        public async Task CreateAsync(Game game, CancellationToken ct = default)
        {
            await _games.AddAsync(game, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<Game>> GetAllAsync(CancellationToken ct = default)
        {
            return await _games.GetAllAsync(ct);
        }
    }
}
