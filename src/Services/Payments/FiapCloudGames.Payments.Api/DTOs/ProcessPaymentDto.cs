namespace FiapCloudGames.Payments.Api.DTOs;

public class ProcessPaymentDto
{
    public int UserId { get; set; }
    public int GameId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
}
