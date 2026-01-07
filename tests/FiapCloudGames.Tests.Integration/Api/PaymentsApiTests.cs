using FiapCloudGames.Payments.Api.DTOs;
using Xunit;

namespace FiapCloudGames.Tests.Integration.Api;

public class PaymentsApiTests
{
    [Fact]
    public void ProcessPaymentDto_ValidDto_HasCorrectProperties()
    {
        var dto = new ProcessPaymentDto
        {
            UserId = 1,
            GameId = 1,
            Amount = 59.99m,
            PaymentMethod = "CreditCard"
        };

        Assert.Equal(1, dto.UserId);
        Assert.Equal(1, dto.GameId);
        Assert.Equal(59.99m, dto.Amount);
        Assert.Equal("CreditCard", dto.PaymentMethod);
    }

    [Fact]
    public void ProcessPaymentDto_EmptyValues_AllowsCreation()
    {
        var dto = new ProcessPaymentDto
        {
            UserId = 0,
            GameId = 0,
            Amount = 0,
            PaymentMethod = ""
        };

        Assert.Equal(0, dto.UserId);
        Assert.Equal(0, dto.GameId);
        Assert.Equal(0, dto.Amount);
        Assert.Empty(dto.PaymentMethod);
    }

    [Theory]
    [InlineData("CreditCard")]
    [InlineData("DebitCard")]
    [InlineData("Pix")]
    [InlineData("Boleto")]
    public void ProcessPaymentDto_DifferentPaymentMethods_SetsCorrectly(string paymentMethod)
    {
        var dto = new ProcessPaymentDto
        {
            UserId = 1,
            GameId = 1,
            Amount = 59.99m,
            PaymentMethod = paymentMethod
        };

        Assert.Equal(paymentMethod, dto.PaymentMethod);
    }
}
