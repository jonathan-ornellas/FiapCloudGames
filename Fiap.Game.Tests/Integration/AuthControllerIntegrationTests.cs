using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Fiap.Game.Api.Controller;
using Fiap.Game.Api.Contracts.Request;
using Fiap.Game.Domain.Entities;
using Fiap.Game.Domain.Enum;
using Fiap.Game.Domain.ValueObjects;
using Fiap.Game.Infra.Data;
using Fiap.Game.Infra.Data.Repository;
using Fiap.Game.Business.Service;
using Fiap.Game.Infra.CrossCutting;

namespace Fiap.Game.Tests.Integration
{
    public class AuthControllerIntegrationTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;

        public AuthControllerIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"Jwt:Key", "test-key-super-secret-for-testing-only-must-be-at-least-32-characters"},
                    {"Jwt:Issuer", "TestIssuer"},
                    {"Jwt:Audience", "TestAudience"}
                })
                .Build();

            var userRepository = new UserRepository(_context);
            var passwordHasher = new PasswordHasherService();
            var tokenService = new JwtTokenService(configuration);
            var unitOfWork = new UnitOfWork(_context);

            _authService = new AuthService(userRepository, passwordHasher, tokenService, unitOfWork);
        }

        [Fact]
        public async Task Given_ValidUserData_When_RegisterAndLogin_Then_ShouldReturnToken()
        {
            var email = new Email("test@example.com");
            var password = new Password("TestPass@123");
            var hashedPassword = new PasswordHasherService().Hash(password.Value);
            
            var user = new User("Test User", email, hashedPassword);

            var registerToken = await _authService.RegisterAsync(user);
            registerToken.Should().NotBeNullOrEmpty();

            var loginToken = await _authService.LoginAsync("test@example.com", "TestPass@123");
            loginToken.Should().NotBeNullOrEmpty();

            var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Email.Value == email.Value);
            userFromDb.Should().NotBeNull();
            userFromDb!.Name.Should().Be("Test User");
            userFromDb.Role.Should().Be(Role.User);
        }

        [Fact]
        public async Task Given_InvalidCredentials_When_Login_Then_ShouldThrowException()
        {
            var email = new Email("test@example.com");
            var password = new Password("TestPass@123");
            var hashedPassword = new PasswordHasherService().Hash(password.Value);
            
            var user = new User("Test User", email, hashedPassword);
            await _authService.RegisterAsync(user);

            var act = async () => await _authService.LoginAsync("test@example.com", "WrongPassword");
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task Given_DuplicateEmail_When_Register_Then_ShouldThrowException()
        {
            var email = new Email("test@example.com");
            var password = new Password("TestPass@123");
            var hashedPassword = new PasswordHasherService().Hash(password.Value);
            
            var user1 = new User("Test User 1", email, hashedPassword);
            var user2 = new User("Test User 2", email, hashedPassword);

            await _authService.RegisterAsync(user1);
            
            var act = async () => await _authService.RegisterAsync(user2);
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}

