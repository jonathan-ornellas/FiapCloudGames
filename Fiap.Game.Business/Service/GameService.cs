using Fiap.Game.Domain.Interface;
using Fiap.Game.Domain.Interface.Repository;
using Fiap.Game.Domain.Interface.Service;

namespace Fiap.Game.Business.Service
{
    public class GameService : IGameService
    {
        private IGameRepository _game;
        private IUnitOfWork _uow;
        public GameService(IGameRepository game, IUnitOfWork uow)
        {
            _game = game;
            _uow = uow;
        }
        public async Task ActivateAsync(Guid id, bool active, CancellationToken ct = default)
        {
            var game = await _game.GetAsync(id, ct) ?? throw new KeyNotFoundException();
            game.Activate();
            await _uow.SaveChangesAsync(ct);
        }

        public async Task<Guid> CreateAsync(Domain.Entities.Game game, CancellationToken ct = default)
        {
            await _game.AddAsync(game, ct);
            await _uow.SaveChangesAsync(ct);
            return game.Id;
        }

        public IQueryable<Domain.Entities.Game> ListActive() => _game.QueryActive();
    }
}
