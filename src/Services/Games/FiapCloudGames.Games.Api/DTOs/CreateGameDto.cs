namespace FiapCloudGames.Games.Api.DTOs;

public class CreateGameDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    public int Rating { get; set; }
}
