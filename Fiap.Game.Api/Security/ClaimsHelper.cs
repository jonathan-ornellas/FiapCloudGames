using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Fiap.Game.Api.Security
{
    public static class ClaimsHelper
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var sub = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
                   ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(sub, out var id))
                throw new UnauthorizedAccessException("UserId inválido no token.");

            return id;
        }
    }
}
