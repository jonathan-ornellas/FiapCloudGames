using Fiap.Game.Domain.Entities;
using Fiap.Game.Domain.Record;

namespace Fiap.Game.Domain.Interface.Repository
{
    public interface ILibraryRepository : IGenericRepository<LibraryItem>
    {
        Task<LibraryItem?> FindAsync(Guid userId, Guid gameId, CancellationToken ct = default);
        Task<IReadOnlyList<LibraryView>> ListByUserAsync(Guid userId, CancellationToken ct = default);
    }
}
