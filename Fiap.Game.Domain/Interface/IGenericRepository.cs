using Fiap.Game.Domain.Entities.Base;

namespace Fiap.Game.Domain.Interface
{
    public interface IGenericRepository<T> where T : Entity
    {
        IQueryable<T> Query();              
        Task<T?> GetByIdAsync(object id, CancellationToken ct = default);
        Task AddAsync(T entity, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
        void Update(T entity);
        void Remove(T entity);
    }
}
