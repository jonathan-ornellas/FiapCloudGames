using Xunit;
using System.Net;
using System.Net.Http.Json;

namespace FiapCloudGames.Tests.Integration;

public class UsersApiTests : IAsyncLifetime
{
    private HttpClient _httpClient = null!;
    private const string BaseUrl = "http://localhost:5001";

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
    public async Task Register_WithValidData_ReturnsOk()
    {
        var registerRequest = new
        {
            email = "test@example.com",
            password = "Password123!",
            fullName = "Test User"
        };

        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", registerRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ReturnsBadRequest()
    {
        var registerRequest = new
        {
            email = "invalid-email",
            password = "Password123!",
            fullName = "Test User"
        };

        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", registerRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var registerRequest = new
        {
            email = "login@example.com",
            password = "Password123!",
            fullName = "Login Test"
        };

        await _httpClient.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email = "login@example.com",
            password = "Password123!"
        };

        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(json);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var loginRequest = new
        {
            email = "nonexistent@example.com",
            password = "WrongPassword123!"
        };

        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Swagger_ReturnsOk()
    {
        var response = await _httpClient.GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
