namespace FiapCloudGames.Games.Api.DTOs;

public class PurchaseGameDto
{
    public int GameId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}
