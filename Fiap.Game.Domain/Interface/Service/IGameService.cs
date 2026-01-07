namespace Fiap.Game.Domain.Interface.Service
{
    public interface IGameService
    {
        Task<Guid> CreateAsync(Domain.Entities.Game game, CancellationToken ct = default);
        IQueryable<Entities.Game> ListActive();
        Task ActivateAsync(Guid id, bool active, CancellationToken ct = default);

    }
}
