using FiapCloudGames.Domain;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.ValueObjects;

namespace FiapCloudGames.Users.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IPasswordHasherService _hasher;
        private readonly ITokenService _tokens;
        private readonly IUnitOfWork _uow;

        public AuthService(IUserRepository users, IPasswordHasherService hasher, ITokenService tokens, IUnitOfWork uow)
        {
            _users = users;
            _hasher = hasher;
            _tokens = tokens;
            _uow = uow;
        }

        public async Task<string> LoginAsync(string email, string password, CancellationToken ct = default)
        {
            var emailValue = new Email(email);
            var userDatabase = await _users.GetByEmailAsync(emailValue, ct) 
                ?? throw new UnauthorizedAccessException("Credenciais inválidas");

            if (!_hasher.Verify(password, userDatabase.Password)) 
                throw new UnauthorizedAccessException("Credenciais inválidas");

            return _tokens.CreateToken(userDatabase);
        }

        public async Task<string> RegisterAsync(User user, CancellationToken ct = default)
        {
            if (await _users.EmailExistsAsync(user.Email, ct)) 
                throw new InvalidOperationException("E-mail já cadastrado");

            user.Password = _hasher.Hash(user.Password);

            await _users.AddAsync(user, ct);
            await _uow.SaveChangesAsync(ct);
            return _tokens.CreateToken(user);
        }
    }
}
