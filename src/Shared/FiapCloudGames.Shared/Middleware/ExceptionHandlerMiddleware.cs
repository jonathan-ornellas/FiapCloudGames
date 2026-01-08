using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace FiapCloudGames.Shared.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = GetStatusCode(exception);
        var response = new ErrorResponse
        {
            StatusCode = statusCode,
            Message = GetMessage(exception),
            Details = GetDetails(exception),
            TraceId = context.TraceIdentifier,
            Timestamp = DateTime.UtcNow
        };

        _logger.LogError(exception,
            "Error occurred: {Message} | StatusCode: {StatusCode} | TraceId: {TraceId}",
            exception.Message,
            statusCode,
            context.TraceIdentifier);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }

    private static int GetStatusCode(Exception exception) => exception switch
    {
        ArgumentException or ArgumentNullException => (int)HttpStatusCode.BadRequest,
        InvalidOperationException => (int)HttpStatusCode.BadRequest,
        UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
        KeyNotFoundException => (int)HttpStatusCode.NotFound,
        NotImplementedException => (int)HttpStatusCode.NotImplemented,
        TimeoutException => (int)HttpStatusCode.RequestTimeout,
        _ => (int)HttpStatusCode.InternalServerError
    };

    private static string GetMessage(Exception exception) => exception switch
    {
        ArgumentException or ArgumentNullException => "Invalid request data",
        InvalidOperationException => "Invalid operation",
        UnauthorizedAccessException => "Unauthorized access",
        KeyNotFoundException => "Resource not found",
        NotImplementedException => "Feature not implemented",
        TimeoutException => "Request timeout",
        _ => "An unexpected error occurred"
    };

    private static string? GetDetails(Exception exception)
    {
        return exception switch
        {
            ArgumentException or 
            ArgumentNullException or 
            InvalidOperationException or 
            UnauthorizedAccessException or 
            KeyNotFoundException => exception.Message,
            _ => null
        };
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string TraceId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
