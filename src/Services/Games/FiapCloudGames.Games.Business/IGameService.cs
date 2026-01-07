using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Games.Business
{
    public interface IGameService
    {
        Task CreateAsync(Game game, CancellationToken ct = default);
        Task<IEnumerable<Game>> GetAllAsync(CancellationToken ct = default);
    }
}
