using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Payments.Business
{
    public interface IPaymentService
    {
        Task CreateAsync(Payment payment, CancellationToken ct = default);
        Task ProcessPaymentAsync(Payment payment, CancellationToken ct = default);
    }
}
