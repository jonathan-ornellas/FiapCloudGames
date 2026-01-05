namespace FiapCloudGames.Games.Api.Services;

using FiapCloudGames.Games.Api.DTOs;
using FiapCloudGames.Games.Api.Models;
using FiapCloudGames.Games.Api.Repositories;
using FiapCloudGames.Shared.Elasticsearch;
using FiapCloudGames.Shared.Models;

public interface IGameService
{
    Task<GameResponse> CreateGameAsync(CreateGameRequest request);
    Task<GameResponse?> GetGameByIdAsync(Guid id);
    Task<IEnumerable<GameResponse>> GetAllGamesAsync();
    Task<IEnumerable<GameResponse>> SearchGamesAsync(string query);
    Task<RecommendationResponse> GetRecommendationsAsync(Guid userId);
    Task<GameResponse> UpdateGameAsync(Guid id, UpdateGameRequest request);
    Task<bool> DeleteGameAsync(Guid id);
}

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IElasticsearchService _elasticsearchService;

    public GameService(IGameRepository gameRepository, IElasticsearchService elasticsearchService)
    {
        _gameRepository = gameRepository;
        _elasticsearchService = elasticsearchService;
    }

    public async Task<GameResponse> CreateGameAsync(CreateGameRequest request)
    {
        var game = new Game
        {
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            Genre = request.Genre,
            ReleaseDate = request.ReleaseDate,
            Rating = 0
        };

        var createdGame = await _gameRepository.CreateAsync(game);

        var gameDto = new GameDto
        {
            Id = createdGame.Id,
            Title = createdGame.Title,
            Description = createdGame.Description,
            Price = createdGame.Price,
            Genre = createdGame.Genre,
            ReleaseDate = createdGame.ReleaseDate,
            Rating = createdGame.Rating,
            IndexedAt = DateTime.UtcNow
        };

        await _elasticsearchService.IndexGameAsync(gameDto);

        return MapToResponse(createdGame);
    }

    public async Task<GameResponse?> GetGameByIdAsync(Guid id)
    {
        var game = await _gameRepository.GetByIdAsync(id);
        return game != null ? MapToResponse(game) : null;
    }

    public async Task<IEnumerable<GameResponse>> GetAllGamesAsync()
    {
        var games = await _gameRepository.GetAllAsync();
        return games.Select(MapToResponse);
    }

    public async Task<IEnumerable<GameResponse>> SearchGamesAsync(string query)
    {
        var results = await _elasticsearchService.SearchGamesAsync(query);
        return results.Select(g => new GameResponse
        {
            Id = g.Id,
            Title = g.Title,
            Description = g.Description,
            Price = g.Price,
            Genre = g.Genre,
            ReleaseDate = g.ReleaseDate,
            Rating = g.Rating
        });
    }

    public async Task<RecommendationResponse> GetRecommendationsAsync(Guid userId)
    {
        var recommended = await _elasticsearchService.GetRecommendedGamesAsync(userId);
        var popular = await _elasticsearchService.GetPopularGamesAsync();

        return new RecommendationResponse
        {
            RecommendedGames = recommended.Select(g => new GameResponse
            {
                Id = g.Id,
                Title = g.Title,
                Description = g.Description,
                Price = g.Price,
                Genre = g.Genre,
                ReleaseDate = g.ReleaseDate,
                Rating = g.Rating
            }).ToList(),
            PopularGames = popular
        };
    }

    public async Task<GameResponse> UpdateGameAsync(Guid id, UpdateGameRequest request)
    {
        var game = await _gameRepository.GetByIdAsync(id);
        if (game == null)
            throw new InvalidOperationException("Game not found");

        game.Title = request.Title;
        game.Description = request.Description;
        game.Price = request.Price;
        game.Genre = request.Genre;
        game.Rating = request.Rating;

        var updated = await _gameRepository.UpdateAsync(game);

        var gameDto = new GameDto
        {
            Id = updated.Id,
            Title = updated.Title,
            Description = updated.Description,
            Price = updated.Price,
            Genre = updated.Genre,
            ReleaseDate = updated.ReleaseDate,
            Rating = updated.Rating,
            IndexedAt = DateTime.UtcNow
        };

        await _elasticsearchService.IndexGameAsync(gameDto);

        return MapToResponse(updated);
    }

    public async Task<bool> DeleteGameAsync(Guid id)
    {
        var result = await _gameRepository.DeleteAsync(id);
        if (result)
        {
            await _elasticsearchService.DeleteGameIndexAsync(id);
        }
        return result;
    }

    private GameResponse MapToResponse(Game game)
    {
        return new GameResponse
        {
            Id = game.Id,
            Title = game.Title,
            Description = game.Description,
            Price = game.Price,
            Genre = game.Genre,
            ReleaseDate = game.ReleaseDate,
            Rating = game.Rating
        };
    }
}
