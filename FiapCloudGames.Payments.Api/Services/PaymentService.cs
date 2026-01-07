namespace FiapCloudGames.Payments.Api.Services;

using FiapCloudGames.Payments.Api.DTOs;
using FiapCloudGames.Payments.Api.Models;
using FiapCloudGames.Payments.Api.Repositories;
using FiapCloudGames.EventSourcing;
using FiapCloudGames.Shared.Models;
using FiapCloudGames.Shared.RabbitMQ;
using FiapCloudGames.Shared.Events;

public interface IPaymentService
{
    Task<PaymentResponse> ProcessPaymentAsync(CreatePaymentRequest request);
    Task<PaymentResponse?> GetPaymentByIdAsync(Guid id);
    Task<IEnumerable<PaymentResponse>> GetPaymentsByUserAsync(Guid userId);
    Task<PaymentResponse> UpdatePaymentStatusAsync(Guid id, string status);
}

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IEventStore _eventStore;
    private readonly IRabbitMQPublisher _rabbitMQPublisher;

    public PaymentService(IPaymentRepository paymentRepository, IEventStore eventStore, IRabbitMQPublisher rabbitMQPublisher)
    {
        _paymentRepository = paymentRepository;
        _eventStore = eventStore;
        _rabbitMQPublisher = rabbitMQPublisher;
    }

    public async Task<PaymentResponse> ProcessPaymentAsync(CreatePaymentRequest request)
    {
        var payment = new Payment
        {
            UserId = request.UserId,
            GameId = request.GameId,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            Status = "Pending"
        };

        var createdPayment = await _paymentRepository.CreateAsync(payment);

        var @event = new PaymentProcessedDomainEvent
        {
            AggregateId = createdPayment.Id,
            PaymentId = createdPayment.Id,
            UserId = createdPayment.UserId,
            GameId = createdPayment.GameId,
            Amount = createdPayment.Amount,
            Status = createdPayment.Status,
            Version = 1
        };

        await _eventStore.AppendEventAsync(@event);

        var paymentEvent = new PaymentProcessedEvent
        {
            PaymentId = createdPayment.Id.ToString(),
            UserId = createdPayment.UserId.ToString(),
            GameId = createdPayment.GameId.ToString(),
            Amount = createdPayment.Amount,
            Status = createdPayment.Status,
            Timestamp = DateTime.UtcNow
        };

        await _rabbitMQPublisher.PublishAsync("fiap-games", "payment.processed", paymentEvent);

        return MapToResponse(createdPayment);
    }

    public async Task<PaymentResponse?> GetPaymentByIdAsync(Guid id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        return payment != null ? MapToResponse(payment) : null;
    }

    public async Task<IEnumerable<PaymentResponse>> GetPaymentsByUserAsync(Guid userId)
    {
        var payments = await _paymentRepository.GetByUserIdAsync(userId);
        return payments.Select(MapToResponse);
    }

    public async Task<PaymentResponse> UpdatePaymentStatusAsync(Guid id, string status)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment == null)
            throw new InvalidOperationException("Payment not found");

        payment.Status = status;
        if (status == "Completed")
            payment.ProcessedAt = DateTime.UtcNow;

        var updated = await _paymentRepository.UpdateAsync(payment);

        var @event = new PaymentProcessedDomainEvent
        {
            AggregateId = updated.Id,
            PaymentId = updated.Id,
            UserId = updated.UserId,
            GameId = updated.GameId,
            Amount = updated.Amount,
            Status = updated.Status,
            Version = 2
        };

        await _eventStore.AppendEventAsync(@event);

        if (status == "Completed")
        {
            var paymentEvent = new PaymentProcessedEvent
            {
                PaymentId = updated.Id.ToString(),
                UserId = updated.UserId.ToString(),
                GameId = updated.GameId.ToString(),
                Amount = updated.Amount,
                Status = updated.Status,
                Timestamp = DateTime.UtcNow
            };

            await _rabbitMQPublisher.PublishAsync("fiap-games", "payment.completed", paymentEvent);
        }

        return MapToResponse(updated);
    }

    private PaymentResponse MapToResponse(Payment payment)
    {
        return new PaymentResponse
        {
            Id = payment.Id,
            UserId = payment.UserId,
            GameId = payment.GameId,
            Amount = payment.Amount,
            Status = payment.Status,
            CreatedAt = payment.CreatedAt,
            ProcessedAt = payment.ProcessedAt
        };
    }
}
