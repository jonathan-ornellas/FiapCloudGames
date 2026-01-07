using FiapCloudGames.Domain;
using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Payments.Business
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _payments;
        private readonly IUnitOfWork _uow;

        public PaymentService(IPaymentRepository payments, IUnitOfWork uow)
        {
            _payments = payments;
            _uow = uow;
        }

        public async Task CreateAsync(Payment payment, CancellationToken ct = default)
        {
            if (payment.Amount.Value <= 0)
                throw new ArgumentException("Valor do pagamento deve ser maior que zero");

            await _payments.AddAsync(payment, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task ProcessPaymentAsync(Payment payment, CancellationToken ct = default)
        {
            // Lógica de processamento de pagamento aqui
            await Task.CompletedTask;
        }
    }
}
