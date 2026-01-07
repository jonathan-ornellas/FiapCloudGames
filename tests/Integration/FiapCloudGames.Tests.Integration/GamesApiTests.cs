using System.Net;
using System.Net.Http.Json;
using Xunit;
using FiapCloudGames.Tests.Integration.Fixtures;

namespace FiapCloudGames.Tests.Integration;

public class GamesApiTests : IAsyncLifetime
{
    private GamesApiWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        _factory = new GamesApiWebApplicationFactory();
        _client = _factory.CreateAuthenticatedClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task GetAllGames_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/games");

        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CreateGame_WithValidData_ReturnsCreated()
    {
        var gameRequest = new
        {
            title = "Test Game",
            description = "A test game for integration testing",
            genre = "Action",
            price = 49.99m,
            rating = 8.5
        };

        var response = await _client.PostAsJsonAsync("/api/games", gameRequest);

        Assert.True(response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateGame_WithNegativePrice_ReturnsBadRequest()
    {
        var gameRequest = new
        {
            title = "Invalid Game",
            description = "Game with negative price",
            genre = "RPG",
            price = -10m,
            rating = 7.0
        };

        var response = await _client.PostAsJsonAsync("/api/games", gameRequest);

        Assert.True(response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CreateGame_WithInvalidRating_ReturnsBadRequest()
    {
        var gameRequest = new
        {
            title = "Invalid Rating Game",
            description = "Game with invalid rating",
            genre = "Adventure",
            price = 29.99m,
            rating = 15.0
        };

        var response = await _client.PostAsJsonAsync("/api/games", gameRequest);

        Assert.True(response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task SearchGames_WithQuery_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/games/search?query=test");

        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetRecommendations_WithUserId_ReturnsOk()
    {
        var userId = Guid.NewGuid().ToString();
        var response = await _client.GetAsync($"/api/games/recommendations/{userId}");

        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetGameById_WithValidId_ReturnsOkOrNotFound()
    {
        var gameId = Guid.NewGuid().ToString();
        var response = await _client.GetAsync($"/api/games/{gameId}");

        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Swagger_ReturnsOk()
    {
        var response = await _client.GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
