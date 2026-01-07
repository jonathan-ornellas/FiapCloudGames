using FiapCloudGames.Domain.ValueObjects;

namespace FiapCloudGames.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GameId { get; set; }
        public Money Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }

        public Payment(int userId, int gameId, Money amount, string paymentMethod)
        {
            UserId = userId;
            GameId = gameId;
            Amount = amount;
            PaymentMethod = paymentMethod;
            Status = "Pending";
        }
    }
}
