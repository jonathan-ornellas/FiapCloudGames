namespace Fiap.Game.Domain.Record
{
    public sealed record LibraryView(Guid GameId,string Title, decimal PricePaid,DateTime PurchasedAt);
}
