using Fiap.Game.Domain.Interface;

namespace Fiap.Game.Infra.Data
{
    public class UnitOfWork(AppDbContext db) : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
    }
}
