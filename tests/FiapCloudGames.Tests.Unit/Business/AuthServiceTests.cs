using FiapCloudGames.Domain;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.ValueObjects;
using FiapCloudGames.Users.Business;
using FiapCloudGames.Users.Business.Services;
using Moq;
using Xunit;

namespace FiapCloudGames.Tests.Unit.Business;

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
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task RegisterAsync_ValidUser_ReturnsToken()
    {
        var user = new User("Test User", new Email("test@example.com"), "password123");

        _userRepositoryMock
            .Setup(x => x.EmailExistsAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        _passwordHasherMock
            .Setup(x => x.Hash(user.Password))
            .Returns("hashedPassword");
        
        _tokenServiceMock
            .Setup(x => x.CreateToken(user))
            .Returns("token123");

        var token = await _authService.RegisterAsync(user);

        Assert.Equal("token123", token);
        Assert.Equal("hashedPassword", user.Password);
        _userRepositoryMock.Verify(x => x.AddAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_EmailAlreadyExists_ThrowsInvalidOperationException()
    {
        var user = new User("Test User", new Email("existing@example.com"), "password123");

        _userRepositoryMock
            .Setup(x => x.EmailExistsAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _authService.RegisterAsync(user)
        );

        Assert.Equal("E-mail já cadastrado", exception.Message);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        var email = "test@example.com";
        var password = "password123";
        var hashedPassword = "hashedPassword";
        var user = new User("Test User", new Email(email), hashedPassword);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        _passwordHasherMock
            .Setup(x => x.Verify(password, hashedPassword))
            .Returns(true);
        
        _tokenServiceMock
            .Setup(x => x.CreateToken(user))
            .Returns("token123");

        var token = await _authService.LoginAsync(email, password);

        Assert.Equal("token123", token);
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_ThrowsUnauthorizedAccessException()
    {
        var email = "notfound@example.com";
        var password = "password123";

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(email, password)
        );

        Assert.Equal("Credenciais inválidas", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        var email = "test@example.com";
        var password = "wrongpassword";
        var user = new User("Test User", new Email(email), "hashedPassword");

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        _passwordHasherMock
            .Setup(x => x.Verify(password, user.Password))
            .Returns(false);

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(email, password)
        );

        Assert.Equal("Credenciais inválidas", exception.Message);
    }
}
