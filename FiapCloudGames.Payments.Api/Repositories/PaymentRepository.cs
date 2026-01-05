namespace FiapCloudGames.Payments.Api.Repositories;

using FiapCloudGames.Payments.Api.Data;
using FiapCloudGames.Payments.Api.Models;
using Microsoft.EntityFrameworkCore;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id);
    Task<Payment> CreateAsync(Payment payment);
    Task<Payment> UpdateAsync(Payment payment);
    Task<IEnumerable<Payment>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Payment>> GetByStatusAsync(string status);
}

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentsContext _context;

    public PaymentRepository(PaymentsContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(Guid id)
    {
        return await _context.Payments.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Payment> CreateAsync(Payment payment)
    {
        payment.Id = Guid.NewGuid();
        payment.CreatedAt = DateTime.UtcNow;

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        return payment;
    }

    public async Task<Payment> UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync();
        return payment;
    }

    public async Task<IEnumerable<Payment>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Payments
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetByStatusAsync(string status)
    {
        return await _context.Payments
            .Where(p => p.Status == status)
            .ToListAsync();
    }
}
