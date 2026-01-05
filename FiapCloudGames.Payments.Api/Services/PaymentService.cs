namespace FiapCloudGames.Payments.Api.Services;

using FiapCloudGames.Payments.Api.DTOs;
using FiapCloudGames.Payments.Api.Models;
using FiapCloudGames.Payments.Api.Repositories;
using FiapCloudGames.EventSourcing;
using FiapCloudGames.Shared.Models;

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

    public PaymentService(IPaymentRepository paymentRepository, IEventStore eventStore)
    {
        _paymentRepository = paymentRepository;
        _eventStore = eventStore;
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

        var @event = new PaymentProcessedEvent
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

        var @event = new PaymentProcessedEvent
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
