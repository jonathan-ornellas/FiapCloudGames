using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Users.Business
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
