using Nest;
using Amazon.Lambda.Core;
using FiapCloudGames.Shared.Models;

namespace FiapCloudGames.Lambda.Services;

public interface IRecommendationService
{
    Task<List<GameDto>> GetRecommendedGamesAsync(Guid userId, int limit = 10);
    Task<List<GameDto>> GetPopularGamesAsync(int limit = 5);
}

public class RecommendationService : IRecommendationService
{
    private readonly IElasticClient _elasticClient;

    public RecommendationService(string elasticsearchUrl = "http://localhost:9200")
    {
        var settings = new ConnectionSettings(new Uri(elasticsearchUrl))
            .DefaultIndex("games");
        _elasticClient = new ElasticClient(settings);
    }

    public async Task<List<GameDto>> GetRecommendedGamesAsync(Guid userId, int limit = 10)
    {
        try
        {
            var response = await _elasticClient.SearchAsync<GameDto>(s => s
                .Index("games")
                .Size(limit)
                .Sort(sort => sort
                    .Descending(g => g.Rating)
                    .Descending(g => g.IndexedAt)
                )
            );

            if (!response.IsValid)
            {
                LambdaLogger.Log($"Elasticsearch error: {response.ServerError?.Error?.Reason}");
                return new List<GameDto>();
            }

            return response.Documents.ToList();
        }
        catch (Exception ex)
        {
            LambdaLogger.Log($"Error getting recommendations: {ex.Message}");
            return new List<GameDto>();
        }
    }

    public async Task<List<GameDto>> GetPopularGamesAsync(int limit = 5)
    {
        try
        {
            var response = await _elasticClient.SearchAsync<GameDto>(s => s
                .Index("games")
                .Size(limit)
                .Query(q => q
                    .MatchAll()
                )
                .Sort(sort => sort
                    .Descending(g => g.Rating)
                )
            );

            if (!response.IsValid)
            {
                LambdaLogger.Log($"Elasticsearch error: {response.ServerError?.Error?.Reason}");
                return new List<GameDto>();
            }

            return response.Documents.ToList();
        }
        catch (Exception ex)
        {
            LambdaLogger.Log($"Error getting popular games: {ex.Message}");
            return new List<GameDto>();
        }
    }
}
