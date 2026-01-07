using FiapCloudGames.Domain;
using FiapCloudGames.Users.Api.Data;

namespace FiapCloudGames.Users.Api.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UsersContext _context;

        public UnitOfWork(UsersContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }
    }
}
