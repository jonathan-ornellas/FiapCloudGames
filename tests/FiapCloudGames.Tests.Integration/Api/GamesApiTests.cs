using FiapCloudGames.Games.Api.DTOs;
using Xunit;

namespace FiapCloudGames.Tests.Integration.Api;

public class GamesApiTests
{
    [Fact]
    public void CreateGameDto_ValidDto_HasCorrectProperties()
    {
        var dto = new CreateGameDto
        {
            Title = "Test Game",
            Description = "A test game",
            Category = "Action",
            Price = 59.99m,
            Rating = 8
        };

        Assert.Equal("Test Game", dto.Title);
        Assert.Equal("A test game", dto.Description);
        Assert.Equal("Action", dto.Category);
        Assert.Equal(59.99m, dto.Price);
        Assert.Equal(8, dto.Rating);
    }

    [Fact]
    public void CreateGameDto_EmptyValues_AllowsCreation()
    {
        var dto = new CreateGameDto
        {
            Title = "",
            Description = "",
            Category = "",
            Price = 0,
            Rating = 0
        };

        Assert.Empty(dto.Title);
        Assert.Empty(dto.Description);
        Assert.Empty(dto.Category);
        Assert.Equal(0, dto.Price);
        Assert.Equal(0, dto.Rating);
    }
}
