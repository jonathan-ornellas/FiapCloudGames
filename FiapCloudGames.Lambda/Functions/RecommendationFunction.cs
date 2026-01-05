namespace FiapCloudGames.Lambda.Functions;

using Amazon.Lambda.Core;
using Newtonsoft.Json;

public class RecommendationFunction
{
    public async Task<string> GenerateRecommendationsAsync(Dictionary<string, object> input, ILambdaContext context)
    {
        context.Logger.LogLine("Generating game recommendations");

        try
        {
            var userId = input.ContainsKey("userId") ? input["userId"].ToString() : null;
            var limit = input.ContainsKey("limit") ? int.Parse(input["limit"].ToString()!) : 10;

            context.Logger.LogLine($"Generating recommendations for user: {userId}, limit: {limit}");

            var recommendations = new
            {
                UserId = userId,
                RecommendedGames = new List<object>
                {
                    new { GameId = Guid.NewGuid(), Title = "Game 1", Score = 0.95 },
                    new { GameId = Guid.NewGuid(), Title = "Game 2", Score = 0.87 },
                    new { GameId = Guid.NewGuid(), Title = "Game 3", Score = 0.76 }
                },
                GeneratedAt = DateTime.UtcNow
            };

            await Task.Delay(200);

            return JsonConvert.SerializeObject(new { success = true, data = recommendations });
        }
        catch (Exception ex)
        {
            context.Logger.LogLine($"Error: {ex.Message}");
            throw;
        }
    }
}
