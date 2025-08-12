using Fiap.Game.Domain.Entities;
using Fiap.Game.Domain.Interface;
using Fiap.Game.Domain.Interface.Repository;
using Fiap.Game.Domain.Interface.Service;
using Fiap.Game.Domain.Record;

namespace Fiap.Game.Business.Service
{
    public class LibraryService : ILibraryService
    {
        private ILibraryRepository _lib;
        private IGameRepository _game;
        private IUnitOfWork _uow;

        public LibraryService(ILibraryRepository lib, IGameRepository game, IUnitOfWork uow)
        {
            _lib = lib;
            _game = game;
            _uow = uow;
        }
        public Task<IReadOnlyList<LibraryView>> ListAsync(Guid userId, CancellationToken ct = default) =>
            _lib.ListByUserAsync(userId, ct);

        public async Task PurchaseAsync(Guid userId, Guid gameId, CancellationToken ct = default)
        {
            var game = await _game.GetAsync(gameId, ct) ?? throw new ArgumentException("Jogo inválido");
            if (!game.IsActive) throw new InvalidOperationException("Jogo inativo");

            var exists = await _lib.FindAsync(userId, gameId, ct);
            if (exists is not null) throw new InvalidOperationException("Já comprado");

            var item = new LibraryItem
            {
                UserId = userId,
                GameId = gameId,
                PricePaid = game.Price
            };

            await _lib.AddAsync(item, ct);
            await _uow.SaveChangesAsync(ct);
        }
    }
}
