using Xunit;
using System.Net;
using System.Net.Http.Json;

namespace FiapCloudGames.Tests.Integration;

public class GamesApiTests : IAsyncLifetime
{
    private HttpClient _httpClient = null!;
    private const string BaseUrl = "http://localhost:5002";

    public async Task InitializeAsync()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        await Task.Delay(2000);
    }

    public Task DisposeAsync()
    {
        _httpClient?.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetAllGames_ReturnsOk()
    {
        var response = await _httpClient.GetAsync("/api/games");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateGame_WithValidData_ReturnsCreated()
    {
        var gameRequest = new
        {
            title = "Test Game",
            description = "A test game",
            genre = "Action",
            price = 49.99m,
            rating = 8.5
        };

        var response = await _httpClient.PostAsJsonAsync("/api/games", gameRequest);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateGame_WithInvalidPrice_ReturnsBadRequest()
    {
        var gameRequest = new
        {
            title = "Invalid Game",
            description = "Invalid price",
            genre = "RPG",
            price = -10m,
            rating = 7.0
        };

        var response = await _httpClient.PostAsJsonAsync("/api/games", gameRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SearchGames_WithQuery_ReturnsResults()
    {
        var response = await _httpClient.GetAsync("/api/games/search?query=test");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetRecommendations_WithUserId_ReturnsOk()
    {
        var response = await _httpClient.GetAsync("/api/games/recommendations/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Swagger_ReturnsOk()
    {
        var response = await _httpClient.GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
