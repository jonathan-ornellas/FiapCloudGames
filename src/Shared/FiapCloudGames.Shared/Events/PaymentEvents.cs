namespace FiapCloudGames.Shared.Events;

public class PaymentProcessedEvent
{
    public string PaymentId { get; set; }
    public string UserId { get; set; }
    public string GameId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public DateTime Timestamp { get; set; }
}

public class GameAddedToLibraryEvent
{
    public string LibraryId { get; set; }
    public string UserId { get; set; }
    public string GameId { get; set; }
    public DateTime AddedAt { get; set; }
}
