using Fiap.Game.Domain.Entities;

namespace Fiap.Game.Domain.Interface.Service
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(User user, CancellationToken ct = default);
        Task<string> LoginAsync(string email, string password, CancellationToken ct = default);
    }
}
