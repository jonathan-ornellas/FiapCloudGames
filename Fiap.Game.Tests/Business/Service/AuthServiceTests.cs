using Fiap.Game.Business.Service;
using Fiap.Game.Domain.Entities;
using Fiap.Game.Domain.Interface;
using Fiap.Game.Domain.Interface.Repository;
using Fiap.Game.Domain.Interface.Service;
using Fiap.Game.Domain.ValueObjects;

namespace Fiap.Game.Tests.Business.Service
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasherService> _passwordHasherMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasherService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _authService = new AuthService(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _tokenServiceMock.Object,
                _unitOfWorkMock.Object);
        }

        public class RegisterAsync : AuthServiceTests
        {
            [Fact]
            public async Task Given_ValidUser_When_RegisterAsync_Then_ShouldReturnToken()
            {
                // Given (Arrange)
                var user = new User("João Silva", new Email("joao@exemplo.com"), "hashedPassword123");
                var expectedToken = "jwt-token-123";

                _userRepositoryMock
                    .Setup(x => x.EmailExistsAsync(user.Email, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(false);

                _userRepositoryMock
                    .Setup(x => x.AddAsync(user, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                _unitOfWorkMock
                    .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

                _tokenServiceMock
                    .Setup(x => x.CreateToken(user))
                    .Returns(expectedToken);

                // When (Act)
                var result = await _authService.RegisterAsync(user);

                // Then (Assert)
                result.Should().Be(expectedToken);
                _userRepositoryMock.Verify(x => x.AddAsync(user, It.IsAny<CancellationToken>()), Times.Once);
                _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            }

            [Fact]
            public async Task Given_ExistingEmail_When_RegisterAsync_Then_ShouldThrowInvalidOperationException()
            {
                // Given (Arrange)
                var user = new User("João Silva", new Email("joao@exemplo.com"), "hashedPassword123");

                _userRepositoryMock
                    .Setup(x => x.EmailExistsAsync(user.Email, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true);

                // When (Act)
                var action = async () => await _authService.RegisterAsync(user);

                // Then (Assert)
                await action.Should().ThrowAsync<InvalidOperationException>()
                    .WithMessage("E-mail já cadastrado");

                _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
                _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            }
        }

        public class LoginAsync : AuthServiceTests
        {
            [Fact]
            public async Task Given_ValidCredentials_When_LoginAsync_Then_ShouldReturnToken()
            {
                // Given (Arrange)
                var email = "joao@exemplo.com";
                var password = "MinhaSenh@123";
                var hashedPassword = "hashedPassword123";
                var expectedToken = "jwt-token-123";

                var user = new User("João Silva", new Email(email), hashedPassword);
                var emailValue = new Email(email);

                _userRepositoryMock
                    .Setup(x => x.GetByEmailAsync(emailValue, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(user);

                _passwordHasherMock
                    .Setup(x => x.Verify(password, hashedPassword))
                    .Returns(true);

                _tokenServiceMock
                    .Setup(x => x.CreateToken(user))
                    .Returns(expectedToken);

                // When (Act)
                var result = await _authService.LoginAsync(email, password);

                // Then (Assert)
                result.Should().Be(expectedToken);
            }

            [Fact]
            public async Task Given_NonExistentEmail_When_LoginAsync_Then_ShouldThrowUnauthorizedAccessException()
            {
                // Given (Arrange)
                var email = "inexistente@exemplo.com";
                var password = "MinhaSenh@123";
                var emailValue = new Email(email);

                _userRepositoryMock
                    .Setup(x => x.GetByEmailAsync(emailValue, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((User?)null);

                // When (Act)
                var action = async () => await _authService.LoginAsync(email, password);

                // Then (Assert)
                await action.Should().ThrowAsync<UnauthorizedAccessException>()
                    .WithMessage("Credenciais inválidas");
            }

            [Fact]
            public async Task Given_InvalidPassword_When_LoginAsync_Then_ShouldThrowUnauthorizedAccessException()
            {
                // Given (Arrange)
                var email = "joao@exemplo.com";
                var password = "SenhaErrada123";
                var hashedPassword = "hashedPassword123";

                var user = new User("João Silva", new Email(email), hashedPassword);
                var emailValue = new Email(email);

                _userRepositoryMock
                    .Setup(x => x.GetByEmailAsync(emailValue, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(user);

                _passwordHasherMock
                    .Setup(x => x.Verify(password, hashedPassword))
                    .Returns(false);

                // When (Act)
                var action = async () => await _authService.LoginAsync(email, password);

                // Then (Assert)
                await action.Should().ThrowAsync<UnauthorizedAccessException>()
                    .WithMessage("Credenciais inválidas");

                _tokenServiceMock.Verify(x => x.CreateToken(It.IsAny<User>()), Times.Never);
            }

            [Theory]
            [InlineData("JOAO@EXEMPLO.COM")]
            [InlineData("  joao@exemplo.com  ")]
            [InlineData("Joao@Exemplo.Com")]
            public async Task Given_EmailWithDifferentCasing_When_LoginAsync_Then_ShouldNormalizeAndWork(string inputEmail)
            {
                // Given (Arrange)
                var normalizedEmail = "joao@exemplo.com";
                var password = "MinhaSenh@123";
                var hashedPassword = "hashedPassword123";
                var expectedToken = "jwt-token-123";

                var user = new User("João Silva", new Email(normalizedEmail), hashedPassword);
                var emailValue = new Email(inputEmail);

                _userRepositoryMock
                    .Setup(x => x.GetByEmailAsync(emailValue, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(user);

                _passwordHasherMock
                    .Setup(x => x.Verify(password, hashedPassword))
                    .Returns(true);

                _tokenServiceMock
                    .Setup(x => x.CreateToken(user))
                    .Returns(expectedToken);

                // When (Act)
                var result = await _authService.LoginAsync(inputEmail, password);

                // Then (Assert)
                result.Should().Be(expectedToken);
            }
        }
    }
}

