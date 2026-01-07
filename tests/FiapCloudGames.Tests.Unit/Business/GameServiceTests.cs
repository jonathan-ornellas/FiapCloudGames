using FiapCloudGames.Domain;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.ValueObjects;
using FiapCloudGames.Games.Business;
using Moq;
using Xunit;

namespace FiapCloudGames.Tests.Unit.Business;

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
    public async Task CreateAsync_ValidGame_SavesGame()
    {
        var game = new Game("Test Game", "A test game", "Action", new Money(59.99m), 8.5);

        await _gameService.CreateAsync(game);

        _gameRepositoryMock.Verify(x => x.AddAsync(game, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllGames()
    {
        var games = new List<Game>
        {
            new Game("Game 1", "Description 1", "Action", new Money(49.99m), 8.0),
            new Game("Game 2", "Description 2", "RPG", new Money(39.99m), 7.5)
        };

        _gameRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(games);

        var result = await _gameService.GetAllAsync();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, g => g.Title == "Game 1");
        Assert.Contains(result, g => g.Title == "Game 2");
    }

    [Fact]
    public async Task GetAllAsync_NoGames_ReturnsEmptyList()
    {
        _gameRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Game>());

        var result = await _gameService.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateAsync_MultipleGames_SavesAll()
    {
        var games = new[]
        {
            new Game("Game 1", "Description 1", "Action", new Money(59.99m), 8.5),
            new Game("Game 2", "Description 2", "RPG", new Money(39.99m), 7.0)
        };

        foreach (var game in games)
        {
            await _gameService.CreateAsync(game);
        }

        _gameRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Theory]
    [InlineData(0.01, 0)]
    [InlineData(10.50, 5)]
    [InlineData(99.99, 10)]
    public async Task CreateAsync_VariousValidValues_SavesGame(decimal price, double rating)
    {
        var game = new Game("Test Game", "Description", "Action", new Money(price), rating);

        await _gameService.CreateAsync(game);

        _gameRepositoryMock.Verify(x => x.AddAsync(game, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
