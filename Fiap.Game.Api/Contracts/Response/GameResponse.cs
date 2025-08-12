namespace Fiap.Game.Api.Contracts.Response
{
    public record GameResponse(Guid Id, string Title, string? Description, decimal Price, DateTime? ReleaseDate, bool IsActive);
}
