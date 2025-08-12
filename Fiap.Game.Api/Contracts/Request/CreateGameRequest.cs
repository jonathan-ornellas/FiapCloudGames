namespace Fiap.Game.Api.Contracts.Request
{
    public record CreateGameRequest(string Title, string? Description, decimal Price, DateTime? ReleaseDate);
}
