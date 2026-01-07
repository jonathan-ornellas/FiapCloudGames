namespace FiapCloudGames.Games.Api.DTOs;

public class CreateGameRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Genre { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
}

public class UpdateGameRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Genre { get; set; } = string.Empty;
    public int Rating { get; set; }
}

public class GameResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Genre { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public int Rating { get; set; }
}

public class SearchGameRequest
{
    public string Query { get; set; } = string.Empty;
}

public class RecommendationResponse
{
    public List<GameResponse> RecommendedGames { get; set; } = new();
    public Dictionary<string, int> PopularGames { get; set; } = new();
}
