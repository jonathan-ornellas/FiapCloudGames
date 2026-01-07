using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.ValueObjects;

namespace FiapCloudGames.Users.Business
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(Email email, CancellationToken ct = default);
        Task AddAsync(User user, CancellationToken ct = default);
    }
}
