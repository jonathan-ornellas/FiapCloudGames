using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Games.Business
{
    public interface IGameRepository
    {
        Task AddAsync(Game game, CancellationToken ct = default);
        Task<IEnumerable<Game>> GetAllAsync(CancellationToken ct = default);
    }
}
