using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.ValueObjects;
using FiapCloudGames.Users.Api.Data;
using FiapCloudGames.Users.Business;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Users.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UsersContext _context;

        public UserRepository(UsersContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            _context.Users.Add(user);
            await Task.CompletedTask;
        }

        public async Task<bool> EmailExistsAsync(Email email, CancellationToken ct = default)
        {
            return await _context.Users.AnyAsync(u => u.Email == email, ct);
        }

        public async Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        }
    }
}
