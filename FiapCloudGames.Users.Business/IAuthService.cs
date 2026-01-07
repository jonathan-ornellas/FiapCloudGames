using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Users.Business
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string email, string password, CancellationToken ct = default);
        Task<string> RegisterAsync(User user, CancellationToken ct = default);
    }
}
