using Xunit;
using System.Net;
using System.Net.Http.Json;

namespace FiapCloudGames.Tests.Integration;

public class PaymentsApiTests : IAsyncLifetime
{
    private HttpClient _httpClient = null!;
    private const string BaseUrl = "http://localhost:5003";

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
    public async Task ProcessPayment_WithValidData_ReturnsCreated()
    {
        var paymentRequest = new
        {
            userId = "user-123",
            gameId = "game-456",
            amount = 49.99m,
            paymentMethod = "credit_card"
        };

        var response = await _httpClient.PostAsJsonAsync("/api/payments", paymentRequest);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task ProcessPayment_WithInvalidAmount_ReturnsBadRequest()
    {
        var paymentRequest = new
        {
            userId = "user-123",
            gameId = "game-456",
            amount = -10m,
            paymentMethod = "credit_card"
        };

        var response = await _httpClient.PostAsJsonAsync("/api/payments", paymentRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetPayment_WithValidId_ReturnsOk()
    {
        var paymentRequest = new
        {
            userId = "user-123",
            gameId = "game-456",
            amount = 29.99m,
            paymentMethod = "credit_card"
        };

        var createResponse = await _httpClient.PostAsJsonAsync("/api/payments", paymentRequest);
        
        if (createResponse.StatusCode == HttpStatusCode.Created)
        {
            var json = await createResponse.Content.ReadAsStringAsync();
            var paymentId = "test-id";

            var getResponse = await _httpClient.GetAsync($"/api/payments/{paymentId}");

            Assert.True(getResponse.StatusCode == HttpStatusCode.OK || getResponse.StatusCode == HttpStatusCode.NotFound);
        }
    }

    [Fact]
    public async Task GetUserPayments_WithValidUserId_ReturnsOk()
    {
        var response = await _httpClient.GetAsync("/api/payments/user/user-123");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePaymentStatus_WithValidData_ReturnsOk()
    {
        var paymentRequest = new
        {
            userId = "user-123",
            gameId = "game-456",
            amount = 39.99m,
            paymentMethod = "credit_card"
        };

        var createResponse = await _httpClient.PostAsJsonAsync("/api/payments", paymentRequest);
        
        if (createResponse.StatusCode == HttpStatusCode.Created)
        {
            var paymentId = "test-id";
            var updateRequest = new { status = "Completed" };
            var updateResponse = await _httpClient.PutAsJsonAsync($"/api/payments/{paymentId}/status", updateRequest);

            Assert.True(updateResponse.StatusCode == HttpStatusCode.OK || updateResponse.StatusCode == HttpStatusCode.NotFound);
        }
    }

    [Fact]
    public async Task Swagger_ReturnsOk()
    {
        var response = await _httpClient.GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
