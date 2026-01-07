using System.Text.Json;

namespace Fiap.Game.Api.Middleware
{
    public class ProblemDetailsMiddleware(RequestDelegate next, ILogger<ProblemDetailsMiddleware> logger)
    {
        public async Task Invoke(HttpContext ctx)
        {
            try
            {
                await next(ctx);
            }
            catch (Exception ex)
            {
                var (status, title, detail) = Map(ex);
                logger.LogError(ex, "Unhandled exception: {Title}", title);

                ctx.Response.StatusCode = status;
                ctx.Response.ContentType = "application/problem+json";

                var payload = new
                {
                    type = $"https://httpstatuses.com/{status}",
                    title,
                    status,
                    detail,
                    traceId = ctx.TraceIdentifier
                };

                await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
        }

        private static (int status, string title, string? detail) Map(Exception ex) => ex switch
        {
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized", null),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found", null),
            InvalidOperationException => (StatusCodes.Status409Conflict, "Conflict", ex.Message),
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request", ex.Message),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", null)
        };
    }
}
