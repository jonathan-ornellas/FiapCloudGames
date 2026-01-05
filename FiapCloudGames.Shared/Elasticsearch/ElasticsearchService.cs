namespace FiapCloudGames.Shared.Elasticsearch;

using FiapCloudGames.Shared.Models;
using Nest;

public class ElasticsearchService : IElasticsearchService
{
    private readonly IElasticClient _client;
    private const string GamesIndex = "games";

    public ElasticsearchService(IElasticClient client)
    {
        _client = client;
    }

    public async Task IndexGameAsync(GameDto game)
    {
        await _client.IndexAsync(game, idx => idx
            .Index(GamesIndex)
            .Id(game.Id.ToString())
        );
    }

    public async Task<IEnumerable<GameDto>> SearchGamesAsync(string query)
    {
        var response = await _client.SearchAsync<GameDto>(s => s
            .Index(GamesIndex)
            .Query(q => q
                .MultiMatch(m => m
                    .Fields(f => f
                        .Field(p => p.Title)
                        .Field(p => p.Description)
                        .Field(p => p.Genre)
                    )
                    .Query(query)
                )
            )
        );

        return response.Documents;
    }

    public async Task<IEnumerable<GameDto>> GetRecommendedGamesAsync(Guid userId, int limit = 10)
    {
        var response = await _client.SearchAsync<GameDto>(s => s
            .Index(GamesIndex)
            .Size(limit)
            .Sort(sort => sort
                .Descending(p => p.Rating)
            )
        );

        return response.Documents;
    }

    public async Task<Dictionary<string, int>> GetPopularGamesAsync(int limit = 10)
    {
        var response = await _client.SearchAsync<GameDto>(s => s
            .Index(GamesIndex)
            .Size(0)
            .Aggregations(a => a
                .Terms("popular_games", t => t
                    .Field(p => p.Title)
                    .Size(limit)
                )
            )
        );

        var result = new Dictionary<string, int>();
        var agg = response.Aggregations.Terms("popular_games");
        
        foreach (var bucket in agg.Buckets)
        {
            result[bucket.Key] = (int)bucket.DocCount;
        }

        return result;
    }

    public async Task DeleteGameIndexAsync(Guid gameId)
    {
        await _client.DeleteAsync<GameDto>(gameId.ToString(), idx => idx.Index(GamesIndex));
    }

    public async Task RefreshIndexAsync()
    {
        await _client.Indices.RefreshAsync(GamesIndex);
    }
}
