namespace FiapCloudGames.Lambda.Functions;

using Amazon.Lambda.Core;
using FiapCloudGames.Lambda.Services;
using Newtonsoft.Json;

public class RecommendationFunction
{
    private readonly IRecommendationService _recommendationService;

    public RecommendationFunction()
    {
        var elasticsearchUrl = Environment.GetEnvironmentVariable("ELASTICSEARCH_URL") ?? "http://localhost:9200";
        _recommendationService = new RecommendationService(elasticsearchUrl);
    }

    public async Task<string> GenerateRecommendationsAsync(Dictionary<string, object> input, ILambdaContext context)
    {
        context.Logger.LogLine("Generating game recommendations from Elasticsearch");

        try
        {
            var userId = input.ContainsKey("userId") ? Guid.Parse(input["userId"].ToString()!) : Guid.Empty;
            var limit = input.ContainsKey("limit") ? int.Parse(input["limit"].ToString()!) : 10;

            context.Logger.LogLine($"Generating recommendations for user: {userId}, limit: {limit}");

            var recommendedGames = await _recommendationService.GetRecommendedGamesAsync(userId, limit);
            var popularGames = await _recommendationService.GetPopularGamesAsync(5);

            var recommendations = new
            {
                UserId = userId,
                RecommendedGames = recommendedGames.Select(g => new
                {
                    g.Id,
                    g.Title,
                    g.Description,
                    g.Price,
                    g.Genre,
                    g.Rating
                }).ToList(),
                PopularGames = popularGames.Select(g => new
                {
                    g.Id,
                    g.Title,
                    g.Description,
                    g.Price,
                    g.Genre,
                    g.Rating
                }).ToList(),
                GeneratedAt = DateTime.UtcNow,
                TotalRecommendations = recommendedGames.Count
            };

            var result = new
            {
                success = true,
                data = recommendations,
                timestamp = DateTime.UtcNow
            };

            context.Logger.LogLine($"Recommendations generated: {JsonConvert.SerializeObject(result)}");
            return JsonConvert.SerializeObject(result);
        }
        catch (Exception ex)
        {
            context.Logger.LogLine($"Error generating recommendations: {ex.Message}");
            return JsonConvert.SerializeObject(new
            {
                success = false,
                error = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
