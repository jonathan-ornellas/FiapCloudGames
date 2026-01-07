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
            if (game.Price.Value <= 0)
                throw new ArgumentException("Preço do jogo deve ser maior que zero");

            if (game.Rating < 0 || game.Rating > 10)
                throw new ArgumentException("Rating do jogo deve ser entre 0 e 10");

            await _games.AddAsync(game, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<Game>> GetAllAsync(CancellationToken ct = default)
        {
            return await _games.GetAllAsync(ct);
        }
    }
}
