using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.ValueObjects;
using Xunit;

namespace FiapCloudGames.Tests.Unit.Entities;

public class UserLibraryTests
{
    [Fact]
    public void Constructor_ValidData_CreatesUserLibrary()
    {
        var userId = 1;
        var gameId = 5;
        var purchasePrice = new Money(59.99m);

        var userLibrary = new UserLibrary(userId, gameId, purchasePrice);

        Assert.Equal(userId, userLibrary.UserId);
        Assert.Equal(gameId, userLibrary.GameId);
        Assert.Equal(59.99m, userLibrary.PurchasePrice.Value);
        Assert.True(userLibrary.PurchaseDate <= DateTime.UtcNow);
        Assert.True(userLibrary.PurchaseDate >= DateTime.UtcNow.AddSeconds(-5));
    }

    [Fact]
    public void Constructor_SetsPurchaseDateToUtcNow()
    {
        var beforeCreate = DateTime.UtcNow;
        var userLibrary = new UserLibrary(1, 1, new Money(49.99m));
        var afterCreate = DateTime.UtcNow;

        Assert.InRange(userLibrary.PurchaseDate, beforeCreate, afterCreate);
    }

    [Theory]
    [InlineData(1, 1, 59.99)]
    [InlineData(2, 3, 149.90)]
    [InlineData(100, 200, 0.99)]
    public void Constructor_DifferentValues_CreatesCorrectly(int userId, int gameId, decimal price)
    {
        var userLibrary = new UserLibrary(userId, gameId, new Money(price));

        Assert.Equal(userId, userLibrary.UserId);
        Assert.Equal(gameId, userLibrary.GameId);
        Assert.Equal(price, userLibrary.PurchasePrice.Value);
    }

    [Fact]
    public void PurchaseDate_IsUtcTime()
    {
        var userLibrary = new UserLibrary(1, 1, new Money(59.99m));

        Assert.Equal(DateTimeKind.Utc, userLibrary.PurchaseDate.Kind);
    }
}
