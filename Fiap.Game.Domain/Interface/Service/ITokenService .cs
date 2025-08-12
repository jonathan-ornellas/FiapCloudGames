using Fiap.Game.Domain.Entities;

namespace Fiap.Game.Domain.Interface.Service
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
