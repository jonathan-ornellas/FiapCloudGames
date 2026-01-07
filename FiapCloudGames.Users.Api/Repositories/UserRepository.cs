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
            await _context.Users.AddAsync(new Models.User { Name = user.Name, Email = user.Email.Value, PasswordHash = user.Password }, ct);
        }

        public async Task<bool> EmailExistsAsync(Email email, CancellationToken ct = default)
        {
            return await _context.Users.AnyAsync(u => u.Email == email.Value, ct);
        }

        public async Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email.Value, ct);
            if (user == null) return null;
            return new User(user.Name, new Email(user.Email), user.PasswordHash);
        }
    }
}
