using FiapCloudGames.Shared.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using Xunit;

namespace FiapCloudGames.Tests.Unit.Middleware;

public class ExceptionHandlerMiddlewareTests
{
    private readonly Mock<ILogger<ExceptionHandlerMiddleware>> _loggerMock;

    public ExceptionHandlerMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<ExceptionHandlerMiddleware>>();
    }

    [Fact]
    public async Task InvokeAsync_NoException_CallsNextDelegate()
    {
        var context = new DefaultHttpContext();
        var nextCalled = false;
        RequestDelegate next = (HttpContext hc) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object);

        await middleware.InvokeAsync(context);

        Assert.True(nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_ArgumentException_Returns400()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        RequestDelegate next = (HttpContext hc) => throw new ArgumentException("Invalid argument");

        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object);

        await middleware.InvokeAsync(context);

        Assert.Equal(400, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);
    }

    [Fact]
    public async Task InvokeAsync_UnauthorizedAccessException_Returns401()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        RequestDelegate next = (HttpContext hc) => throw new UnauthorizedAccessException("Unauthorized");

        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object);

        await middleware.InvokeAsync(context);

        Assert.Equal(401, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_KeyNotFoundException_Returns404()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        RequestDelegate next = (HttpContext hc) => throw new KeyNotFoundException("Resource not found");

        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object);

        await middleware.InvokeAsync(context);

        Assert.Equal(404, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_GenericException_Returns500()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        RequestDelegate next = (HttpContext hc) => throw new Exception("Unexpected error");

        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object);

        await middleware.InvokeAsync(context);

        Assert.Equal(500, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_Exception_ReturnsJsonResponse()
    {
        var context = new DefaultHttpContext();
        var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        RequestDelegate next = (HttpContext hc) => throw new ArgumentException("Test error");

        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object);

        await middleware.InvokeAsync(context);

        responseBody.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(responseBody).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseText, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(errorResponse);
        Assert.Equal(400, errorResponse.StatusCode);
        Assert.Equal("Invalid request data", errorResponse.Message);
        Assert.NotNull(errorResponse.TraceId);
    }

    [Fact]
    public async Task InvokeAsync_Exception_LogsError()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new ArgumentException("Test error");
        RequestDelegate next = (HttpContext hc) => throw exception;

        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object);

        await middleware.InvokeAsync(context);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(typeof(InvalidOperationException), 400)]
    [InlineData(typeof(TimeoutException), 408)]
    [InlineData(typeof(NotImplementedException), 501)]
    public async Task InvokeAsync_DifferentExceptions_ReturnsCorrectStatusCode(Type exceptionType, int expectedStatusCode)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = (Exception)Activator.CreateInstance(exceptionType, "Test error")!;
        RequestDelegate next = (HttpContext hc) => throw exception;

        var middleware = new ExceptionHandlerMiddleware(next, _loggerMock.Object);

        await middleware.InvokeAsync(context);

        Assert.Equal(expectedStatusCode, context.Response.StatusCode);
    }
}
