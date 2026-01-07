using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Payments.Business
{
    public interface IPaymentService
    {
        Task CreateAsync(Payment payment, CancellationToken ct = default);
    }
}
