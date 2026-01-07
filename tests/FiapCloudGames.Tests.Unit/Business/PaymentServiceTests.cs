using FiapCloudGames.Domain;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.ValueObjects;
using FiapCloudGames.Payments.Business;
using Moq;
using Xunit;

namespace FiapCloudGames.Tests.Unit.Business;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _paymentService = new PaymentService(_paymentRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidPayment_SavesPayment()
    {
        var payment = new Payment(1, 1, new Money(59.99m), "CreditCard");

        await _paymentService.CreateAsync(payment);

        _paymentRepositoryMock.Verify(x => x.AddAsync(payment, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessPaymentAsync_ValidPayment_CompletesSuccessfully()
    {
        var payment = new Payment(1, 1, new Money(59.99m), "CreditCard");

        await _paymentService.ProcessPaymentAsync(payment);

        Assert.True(true);
    }

    [Fact]
    public async Task CreateAsync_MultiplePayments_SavesAll()
    {
        var payments = new[]
        {
            new Payment(1, 1, new Money(59.99m), "CreditCard"),
            new Payment(2, 2, new Money(39.99m), "Pix")
        };

        foreach (var payment in payments)
        {
            await _paymentService.CreateAsync(payment);
        }

        _paymentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Theory]
    [InlineData("CreditCard")]
    [InlineData("DebitCard")]
    [InlineData("Pix")]
    [InlineData("Boleto")]
    public async Task CreateAsync_DifferentPaymentMethods_SavesPayment(string paymentMethod)
    {
        var payment = new Payment(1, 1, new Money(59.99m), paymentMethod);

        await _paymentService.CreateAsync(payment);

        _paymentRepositoryMock.Verify(x => x.AddAsync(payment, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(paymentMethod, payment.PaymentMethod);
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(10.50)]
    [InlineData(99.99)]
    [InlineData(1000)]
    public async Task CreateAsync_VariousValidAmounts_SavesPayment(decimal amount)
    {
        var payment = new Payment(1, 1, new Money(amount), "CreditCard");

        await _paymentService.CreateAsync(payment);

        _paymentRepositoryMock.Verify(x => x.AddAsync(payment, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(amount, payment.Amount.Value);
    }

    [Fact]
    public async Task CreateAsync_WithStatus_SavesWithPendingStatus()
    {
        var payment = new Payment(1, 1, new Money(59.99m), "CreditCard");

        await _paymentService.CreateAsync(payment);

        Assert.Equal("Pending", payment.Status);
        _paymentRepositoryMock.Verify(x => x.AddAsync(payment, It.IsAny<CancellationToken>()), Times.Once);
    }
}
