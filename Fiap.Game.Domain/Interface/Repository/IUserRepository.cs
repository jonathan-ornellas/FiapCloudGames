using Fiap.Game.Domain.Entities;
using Fiap.Game.Domain.ValueObjects;

namespace Fiap.Game.Domain.Interface.Repository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<bool> EmailExistsAsync(Email email, CancellationToken ct = default);
        Task<User?> GetByEmailAsync(Email email, CancellationToken ct = default);
    }
}
