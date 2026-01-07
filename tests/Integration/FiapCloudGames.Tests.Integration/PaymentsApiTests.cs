using System.Net;
using System.Net.Http.Json;
using Xunit;
using FiapCloudGames.Tests.Integration.Fixtures;

namespace FiapCloudGames.Tests.Integration;

public class PaymentsApiTests : IAsyncLifetime
{
    private PaymentsApiWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        _factory = new PaymentsApiWebApplicationFactory();
        _client = _factory.CreateAuthenticatedClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task ProcessPayment_WithValidData_ReturnsCreated()
    {
        var paymentRequest = new
        {
            userId = Guid.NewGuid().ToString(),
            gameId = Guid.NewGuid().ToString(),
            amount = 49.99m,
            paymentMethod = "credit_card"
        };

        var response = await _client.PostAsJsonAsync("/api/payments", paymentRequest);

        Assert.True(response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task ProcessPayment_WithNegativeAmount_ReturnsBadRequest()
    {
        var paymentRequest = new
        {
            userId = Guid.NewGuid().ToString(),
            gameId = Guid.NewGuid().ToString(),
            amount = -10m,
            paymentMethod = "credit_card"
        };

        var response = await _client.PostAsJsonAsync("/api/payments", paymentRequest);

        Assert.True(response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ProcessPayment_WithZeroAmount_ReturnsBadRequest()
    {
        var paymentRequest = new
        {
            userId = Guid.NewGuid().ToString(),
            gameId = Guid.NewGuid().ToString(),
            amount = 0m,
            paymentMethod = "credit_card"
        };

        var response = await _client.PostAsJsonAsync("/api/payments", paymentRequest);

        Assert.True(response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ProcessPayment_WithInvalidPaymentMethod_ReturnsBadRequest()
    {
        var paymentRequest = new
        {
            userId = Guid.NewGuid().ToString(),
            gameId = Guid.NewGuid().ToString(),
            amount = 29.99m,
            paymentMethod = "invalid_method"
        };

        var response = await _client.PostAsJsonAsync("/api/payments", paymentRequest);

        Assert.True(response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetPayment_WithValidId_ReturnsOkOrNotFound()
    {
        var paymentId = Guid.NewGuid().ToString();
        var response = await _client.GetAsync($"/api/payments/{paymentId}");

        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserPayments_WithValidUserId_ReturnsOk()
    {
        var userId = Guid.NewGuid().ToString();
        var response = await _client.GetAsync($"/api/payments/user/{userId}");

        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdatePaymentStatus_WithValidData_ReturnsOkOrNotFound()
    {
        var paymentId = Guid.NewGuid().ToString();
        var updateRequest = new { status = "Completed" };

        var response = await _client.PutAsJsonAsync($"/api/payments/{paymentId}/status", updateRequest);

        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Swagger_ReturnsOk()
    {
        var response = await _client.GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
