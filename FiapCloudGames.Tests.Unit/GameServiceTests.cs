using FiapCloudGames.Games.Business;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.ValueObjects;
using Moq;
using FluentAssertions;
using FiapCloudGames.Domain;
using Xunit;

namespace FiapCloudGames.Tests.Unit;

public class GameServiceTests
{
    private readonly Mock<IGameRepository> _gameRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly GameService _gameService;

    public GameServiceTests()
    {
        _gameRepositoryMock = new Mock<IGameRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _gameService = new GameService(_gameRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidGame_ShouldSucceed()
    {
        // Arrange
        var game = new Game("Test Game", "Description", "Action", new Money(59.99m), 9.0);

        // Act
        await _gameService.CreateAsync(game);

        // Assert
        _gameRepositoryMock.Verify(r => r.AddAsync(game, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithZeroPrice_ShouldThrowArgumentException()
    {
        // Arrange
        var game = new Game("Test Game", "Description", "Action", new Money(0), 9.0);

        // Act
        Func<Task> act = async () => await _gameService.CreateAsync(game);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Preço do jogo deve ser maior que zero");
    }

    [Fact]
    public async Task CreateAsync_WithNegativePrice_ShouldThrowArgumentException()
    {
        // Arrange
        var game = new Game("Test Game", "Description", "Action", new Money(-10), 9.0);

        // Act
        Func<Task> act = async () => await _gameService.CreateAsync(game);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Valor monetário não pode ser negativo");
    }

    [Fact]
    public async Task CreateAsync_WithInvalidRating_ShouldThrowArgumentException()
    {
        // Arrange
        var game = new Game("Test Game", "Description", "Action", new Money(59.99m), 11.0);

        // Act
        Func<Task> act = async () => await _gameService.CreateAsync(game);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Rating do jogo deve ser entre 0 e 10");
    }
}
