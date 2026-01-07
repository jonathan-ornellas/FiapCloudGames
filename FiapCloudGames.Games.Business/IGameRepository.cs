using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Games.Business
{
    public interface IGameRepository
    {
        Task AddAsync(Game game, CancellationToken ct = default);
    }
}
