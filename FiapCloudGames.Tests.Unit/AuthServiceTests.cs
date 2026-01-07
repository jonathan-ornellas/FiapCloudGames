using FiapCloudGames.Users.Business;
using FiapCloudGames.Users.Business.Services;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.ValueObjects;
using Moq;
using FluentAssertions;
using FiapCloudGames.Domain;
using Xunit;

namespace FiapCloudGames.Tests.Unit;

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

    [Fact]
    public async Task RegisterAsync_WithValidUser_ShouldReturnToken()
    {
        // Arrange
        var user = new User("Test User", new Email("test@example.com"), "Password123!");
        var expectedToken = "test-token";

        _userRepositoryMock.Setup(r => r.EmailExistsAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _passwordHasherMock.Setup(h => h.Hash(user.Password)).Returns("hashed-password");
        _tokenServiceMock.Setup(t => t.CreateToken(user)).Returns(expectedToken);

        // Act
        var result = await _authService.RegisterAsync(user);

        // Assert
        result.Should().Be(expectedToken);
        _userRepositoryMock.Verify(r => r.AddAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user = new User("Test User", new Email("test@example.com"), "Password123!");

        _userRepositoryMock.Setup(r => r.EmailExistsAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _authService.RegisterAsync(user);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("E-mail já cadastrado");
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var user = new User("Test User", new Email(email), "hashed-password");
        var expectedToken = "test-token";

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(new Email(email), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.Verify(password, user.Password)).Returns(true);
        _tokenServiceMock.Setup(t => t.CreateToken(user)).Returns(expectedToken);

        // Act
        var result = await _authService.LoginAsync(email, password);

        // Assert
        result.Should().Be(expectedToken);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(new Email(email), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(email, password);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("Credenciais inválidas");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var user = new User("Test User", new Email(email), "hashed-password");

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(new Email(email), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasherMock.Setup(h => h.Verify(password, user.Password)).Returns(false);

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(email, password);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("Credenciais inválidas");
    }
}
