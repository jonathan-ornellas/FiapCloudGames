using FiapCloudGames.Domain.ValueObjects;
using Xunit;

namespace FiapCloudGames.Tests.Unit.ValueObjects;

public class MoneyTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(0.01)]
    [InlineData(10.50)]
    [InlineData(99.99)]
    [InlineData(1000)]
    [InlineData(9999.99)]
    public void Constructor_ValidAmount_CreatesMoney(decimal amount)
    {
        var money = new Money(amount);

        Assert.Equal(amount, money.Value);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-10)]
    [InlineData(-100.50)]
    public void Constructor_NegativeAmount_ThrowsArgumentException(decimal amount)
    {
        var exception = Assert.Throws<ArgumentException>(() => new Money(amount));

        Assert.Equal("Valor monetário não pode ser negativo (Parameter 'value')", exception.Message);
    }

    [Fact]
    public void Equals_SameAmount_ReturnsTrue()
    {
        var money1 = new Money(59.99m);
        var money2 = new Money(59.99m);

        Assert.True(money1.Equals(money2));
        Assert.True(money1 == money2);
    }

    [Fact]
    public void Equals_DifferentAmount_ReturnsFalse()
    {
        var money1 = new Money(59.99m);
        var money2 = new Money(49.99m);

        Assert.False(money1.Equals(money2));
        Assert.True(money1 != money2);
    }

    [Fact]
    public void ImplicitConversion_ToDecimal_Works()
    {
        var money = new Money(59.99m);
        decimal value = money;

        Assert.Equal(59.99m, value);
    }

    [Fact]
    public void ImplicitConversion_FromDecimal_Works()
    {
        Money money = 59.99m;

        Assert.Equal(59.99m, money.Value);
    }

    [Fact]
    public void ToString_FormatsAsCurrency()
    {
        var money = new Money(59.99m);

        var result = money.ToString();

        Assert.Contains("59", result);
        Assert.Contains("99", result);
    }

    [Fact]
    public void GetHashCode_SameAmount_ReturnsSameHashCode()
    {
        var money1 = new Money(59.99m);
        var money2 = new Money(59.99m);

        Assert.Equal(money1.GetHashCode(), money2.GetHashCode());
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var money = new Money(59.99m);

        Assert.False(money.Equals(null));
    }

    [Fact]
    public void OperatorEquals_BothNull_ReturnsTrue()
    {
        Money? money1 = null;
        Money? money2 = null;

        Assert.True(money1 == money2);
    }

    [Fact]
    public void OperatorEquals_OneNull_ReturnsFalse()
    {
        var money1 = new Money(59.99m);
        Money? money2 = null;

        Assert.False(money1 == money2);
        Assert.False(money2 == money1);
    }

    [Theory]
    [InlineData(0.99)]
    [InlineData(1.00)]
    [InlineData(1.01)]
    public void Constructor_PreciseDecimals_MaintainsPrecision(decimal amount)
    {
        var money = new Money(amount);

        Assert.Equal(amount, money.Value);
    }

    [Fact]
    public void Constructor_MaxDecimalValue_Works()
    {
        var money = new Money(decimal.MaxValue);

        Assert.Equal(decimal.MaxValue, money.Value);
    }

    [Fact]
    public void Constructor_ZeroAmount_Works()
    {
        var money = new Money(0);

        Assert.Equal(0, money.Value);
    }
}
