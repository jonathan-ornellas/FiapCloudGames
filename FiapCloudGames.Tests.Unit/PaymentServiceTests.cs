using FiapCloudGames.Payments.Business;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.ValueObjects;
using Moq;
using FluentAssertions;
using FiapCloudGames.Domain;
using Xunit;

namespace FiapCloudGames.Tests.Unit;

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
    public async Task CreateAsync_WithValidPayment_ShouldSucceed()
    {
        // Arrange
        var payment = new Payment(1, 1, new Money(59.99m), "Credit Card");

        // Act
        await _paymentService.CreateAsync(payment);

        // Assert
        _paymentRepositoryMock.Verify(r => r.AddAsync(payment, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithZeroAmount_ShouldThrowArgumentException()
    {
        // Arrange
        var payment = new Payment(1, 1, new Money(0), "Credit Card");

        // Act
        Func<Task> act = async () => await _paymentService.CreateAsync(payment);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Valor do pagamento deve ser maior que zero");
    }

    [Fact]
    public async Task CreateAsync_WithNegativeAmount_ShouldThrowArgumentException()
    {
        // Arrange
        var payment = new Payment(1, 1, new Money(-10), "Credit Card");

        // Act
        Func<Task> act = async () => await _paymentService.CreateAsync(payment);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Valor monetário não pode ser negativo");
    }
}
