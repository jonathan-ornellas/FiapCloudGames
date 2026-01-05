namespace FiapCloudGames.Shared.Elasticsearch;

using FiapCloudGames.Shared.Models;

/// <summary>
/// Interface para operações com Elasticsearch
/// </summary>
public interface IElasticsearchService
{
    Task IndexGameAsync(GameDto game);
    Task<IEnumerable<GameDto>> SearchGamesAsync(string query);
    Task<IEnumerable<GameDto>> GetRecommendedGamesAsync(Guid userId, int limit = 10);
    Task<Dictionary<string, int>> GetPopularGamesAsync(int limit = 10);
    Task DeleteGameIndexAsync(Guid gameId);
    Task RefreshIndexAsync();
}
