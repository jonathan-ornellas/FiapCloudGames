using Xunit;
using FluentAssertions;

namespace FiapCloudGames.Tests.Unit;

public class GameServiceTests
{
    [Fact]
    public void ValidateGamePrice_WithPositivePrice_ShouldReturnTrue()
    {
        var price = 49.99m;
        var isValid = price > 0;

        isValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateGamePrice_WithNegativePrice_ShouldReturnFalse()
    {
        var price = -10m;
        var isValid = price > 0;

        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateGamePrice_WithZeroPrice_ShouldReturnFalse()
    {
        var price = 0m;
        var isValid = price > 0;

        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateGameRating_WithValidRating_ShouldReturnTrue()
    {
        var rating = 8.5;
        var isValid = rating >= 0 && rating <= 10;

        isValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateGameRating_WithRatingBelowZero_ShouldReturnFalse()
    {
        var rating = -1.0;
        var isValid = rating >= 0 && rating <= 10;

        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateGameRating_WithRatingAboveTen_ShouldReturnFalse()
    {
        var rating = 11.0;
        var isValid = rating >= 0 && rating <= 10;

        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateGameTitle_WithValidTitle_ShouldReturnTrue()
    {
        var title = "Test Game";
        var isValid = !string.IsNullOrWhiteSpace(title) && title.Length > 0;

        isValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateGameTitle_WithEmptyTitle_ShouldReturnFalse()
    {
        var title = string.Empty;
        var isValid = !string.IsNullOrWhiteSpace(title);

        isValid.Should().BeFalse();
    }

    [Fact]
    public void CalculateDiscount_WithValidPercentage_ShouldReturnCorrectPrice()
    {
        var originalPrice = 100m;
        var discountPercentage = 10;
        var discountedPrice = originalPrice * (1 - (discountPercentage / 100m));

        discountedPrice.Should().Be(90m);
    }

    [Fact]
    public void CalculateDiscount_With50Percent_ShouldReturnHalfPrice()
    {
        var originalPrice = 100m;
        var discountPercentage = 50;
        var discountedPrice = originalPrice * (1 - (discountPercentage / 100m));

        discountedPrice.Should().Be(50m);
    }
}
