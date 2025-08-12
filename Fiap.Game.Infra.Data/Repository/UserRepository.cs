using Fiap.Game.Domain.Entities;
using Fiap.Game.Domain.Interface.Repository;
using Fiap.Game.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Game.Infra.Data.Repository
{
    public class UserRepository(AppDbContext db) : Repository<User>(db), IUserRepository
    {

        public async Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default)
        {
            return await db.Users
                .FirstOrDefaultAsync(u => u.Email == email, ct);
        }

        public async Task<bool> EmailExistsAsync(Email email, CancellationToken ct = default)
        {
            return await db.Users
                .AnyAsync(u => u.Email == email, ct);
        }

    }
}
