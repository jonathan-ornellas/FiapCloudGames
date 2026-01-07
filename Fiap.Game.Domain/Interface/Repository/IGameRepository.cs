namespace Fiap.Game.Domain.Interface.Repository
{
    using Fiap.Game.Domain.Entities;

    public interface IGameRepository : IGenericRepository<Game>
    {
        Task<Game?> GetAsync(Guid id, CancellationToken ct = default);
        IQueryable<Game> QueryActive();

    }
}

