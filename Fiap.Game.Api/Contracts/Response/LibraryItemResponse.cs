namespace Fiap.Game.Api.Contracts.Response
{
    public record LibraryItemResponse(Guid GameId, string Title, decimal PricePaid, DateTime PurchasedAt);
}
