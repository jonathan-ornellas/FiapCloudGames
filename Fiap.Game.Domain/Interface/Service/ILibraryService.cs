using Fiap.Game.Domain.Record;

namespace Fiap.Game.Domain.Interface.Service
{
    public interface ILibraryService
    {
        Task PurchaseAsync(Guid userId, Guid gameId, CancellationToken ct = default);
        Task<IReadOnlyList<LibraryView>> ListAsync(Guid userId, CancellationToken ct = default);
    }
}
