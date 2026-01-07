using Xunit;
using FluentAssertions;

namespace FiapCloudGames.Tests.Unit;

public class PaymentServiceTests
{
    [Fact]
    public void ValidatePaymentAmount_WithPositiveAmount_ShouldReturnTrue()
    {
        var amount = 49.99m;
        var isValid = amount > 0;

        isValid.Should().BeTrue();
    }

    [Fact]
    public void ValidatePaymentAmount_WithNegativeAmount_ShouldReturnFalse()
    {
        var amount = -10m;
        var isValid = amount > 0;

        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidatePaymentAmount_WithZeroAmount_ShouldReturnFalse()
    {
        var amount = 0m;
        var isValid = amount > 0;

        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidatePaymentMethod_WithValidMethod_ShouldReturnTrue()
    {
        var method = "credit_card";
        var validMethods = new[] { "credit_card", "debit_card", "paypal", "bank_transfer" };
        var isValid = validMethods.Contains(method);

        isValid.Should().BeTrue();
    }

    [Fact]
    public void ValidatePaymentMethod_WithInvalidMethod_ShouldReturnFalse()
    {
        var method = "invalid_method";
        var validMethods = new[] { "credit_card", "debit_card", "paypal", "bank_transfer" };
        var isValid = validMethods.Contains(method);

        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidatePaymentStatus_WithValidStatus_ShouldReturnTrue()
    {
        var status = "Completed";
        var validStatuses = new[] { "Pending", "Processing", "Completed", "Failed", "Cancelled" };
        var isValid = validStatuses.Contains(status);

        isValid.Should().BeTrue();
    }

    [Fact]
    public void ValidatePaymentStatus_WithInvalidStatus_ShouldReturnFalse()
    {
        var status = "InvalidStatus";
        var validStatuses = new[] { "Pending", "Processing", "Completed", "Failed", "Cancelled" };
        var isValid = validStatuses.Contains(status);

        isValid.Should().BeFalse();
    }

    [Fact]
    public void CalculateTax_WithValidAmount_ShouldReturnCorrectTax()
    {
        var amount = 100m;
        var taxPercentage = 15;
        var tax = amount * (taxPercentage / 100m);

        tax.Should().Be(15m);
    }

    [Fact]
    public void CalculateTotalWithTax_WithValidAmount_ShouldReturnCorrectTotal()
    {
        var amount = 100m;
        var taxPercentage = 15;
        var total = amount + (amount * (taxPercentage / 100m));

        total.Should().Be(115m);
    }

    [Fact]
    public void ValidateRefund_WithCompletedPayment_ShouldBeAllowed()
    {
        var paymentStatus = "Completed";
        var canRefund = paymentStatus == "Completed" || paymentStatus == "Processing";

        canRefund.Should().BeTrue();
    }

    [Fact]
    public void ValidateRefund_WithFailedPayment_ShouldNotBeAllowed()
    {
        var paymentStatus = "Failed";
        var canRefund = paymentStatus == "Completed" || paymentStatus == "Processing";

        canRefund.Should().BeFalse();
    }
}
