using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Payments.Business
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment, CancellationToken ct = default);
    }
}
