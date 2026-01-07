using Fiap.Game.Domain.Entities.Base;
using Fiap.Game.Domain.Interface;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Game.Infra.Data.Repository
{
    public class Repository<T>(AppDbContext db) : IGenericRepository<T> where T : Entity
    {
        protected readonly AppDbContext _db = db;
        protected DbSet<T> Set => _db.Set<T>();
        public Task AddAsync(T entity, CancellationToken ct = default)
            => Set.AddAsync(entity, ct).AsTask();
        public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
            => Set.AddRangeAsync(entities, ct);
        public Task<T?> GetByIdAsync(object id, CancellationToken ct = default)
            => Set.FindAsync([id], ct).AsTask();
        public IQueryable<T> Query() => Set.AsQueryable();
        public void Update(T entity) => Set.Update(entity);
        public void Remove(T entity) => Set.Remove(entity);
    }
}
