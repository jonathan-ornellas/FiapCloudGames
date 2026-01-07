using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.ValueObjects;

namespace FiapCloudGames.Users.Business
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default);
        Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(Email email, CancellationToken ct = default);
        Task AddAsync(User user, CancellationToken ct = default);
        Task UpdateAsync(User user, CancellationToken ct = default);
        Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default);
    }
}
