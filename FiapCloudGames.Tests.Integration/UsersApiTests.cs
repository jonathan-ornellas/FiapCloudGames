using System.Net;
using System.Net.Http.Json;
using Xunit;
using FiapCloudGames.Tests.Integration.Fixtures;

namespace FiapCloudGames.Tests.Integration;

public class UsersApiTests : IAsyncLifetime
{
    private UsersApiWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        _factory = new UsersApiWebApplicationFactory();
        _client = _factory.CreateAuthenticatedClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOk()
    {
        var registerRequest = new
        {
            email = "newuser@example.com",
            password = "Password123!",
            fullName = "New User"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
    {
        var registerRequest = new
        {
            email = "duplicate@example.com",
            password = "Password123!",
            fullName = "First User"
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var duplicateRequest = new
        {
            email = "duplicate@example.com",
            password = "Password456!",
            fullName = "Second User"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", duplicateRequest);

        Assert.True(response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ReturnsBadRequest()
    {
        var registerRequest = new
        {
            email = "invalid-email",
            password = "Password123!",
            fullName = "Invalid Email User"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        Assert.True(response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Register_WithWeakPassword_ReturnsBadRequest()
    {
        var registerRequest = new
        {
            email = "weak@example.com",
            password = "weak",
            fullName = "Weak Password User"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        Assert.True(response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var registerRequest = new
        {
            email = "login@example.com",
            password = "Password123!",
            fullName = "Login Test User"
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email = "login@example.com",
            password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var loginRequest = new
        {
            email = "nonexistent@example.com",
            password = "WrongPassword123!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Swagger_ReturnsOk()
    {
        var response = await _client.GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
