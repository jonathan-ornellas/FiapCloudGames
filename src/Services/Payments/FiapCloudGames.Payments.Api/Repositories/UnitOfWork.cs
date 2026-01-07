using FiapCloudGames.Domain;
using FiapCloudGames.Payments.Api.Data;

namespace FiapCloudGames.Payments.Api.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PaymentsContext _context;

        public UnitOfWork(PaymentsContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }
    }
}
