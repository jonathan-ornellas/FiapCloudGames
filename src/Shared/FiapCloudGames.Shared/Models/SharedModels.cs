namespace FiapCloudGames.Shared.Models;

/// <summary>
/// Modelo de resposta padrão para APIs
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
}

/// <summary>
/// Modelo de usuário para comunicação entre microsserviços
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Modelo de jogo para comunicação entre microsserviços
/// </summary>
public class GameDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Genre { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public int Rating { get; set; }
    public DateTime IndexedAt { get; set; }
}

/// <summary>
/// Modelo de transação de pagamento
/// </summary>
public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

/// <summary>
/// Evento de domínio para Event Sourcing
/// </summary>
public abstract class DomainEvent
{
    public Guid AggregateId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = string.Empty;
    public int Version { get; set; }
}

/// <summary>
/// Evento de usuário criado
/// </summary>
public class UserCreatedEvent : DomainEvent
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Evento de jogo comprado
/// </summary>
public class GamePurchasedEvent : DomainEvent
{
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public decimal Amount { get; set; }
}

/// <summary>
/// Evento de pagamento processado para Event Sourcing
/// </summary>
public class PaymentProcessedDomainEvent : DomainEvent
{
    public Guid PaymentId { get; set; }
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
}
