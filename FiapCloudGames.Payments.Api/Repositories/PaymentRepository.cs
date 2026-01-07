using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Payments.Api.Data;
using FiapCloudGames.Payments.Business;

namespace FiapCloudGames.Payments.Api.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentsContext _context;

        public PaymentRepository(PaymentsContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Payment payment, CancellationToken ct = default)
        {
                        await _context.Payments.AddAsync(new Models.Payment { UserId = Guid.Parse(payment.UserId.ToString()), GameId = Guid.Parse(payment.GameId.ToString()), Amount = payment.Amount.Value, PaymentMethod = payment.PaymentMethod, Status = payment.Status }, ct);
        }
    }
}
