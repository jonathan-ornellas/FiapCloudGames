using FiapCloudGames.Domain.ValueObjects;

namespace FiapCloudGames.Domain.Entities
{
    public class UserLibrary
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GameId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public Money PurchasePrice { get; set; }

        public UserLibrary(int userId, int gameId, Money purchasePrice)
        {
            UserId = userId;
            GameId = gameId;
            PurchasePrice = purchasePrice;
            PurchaseDate = DateTime.UtcNow;
        }
    }
}
