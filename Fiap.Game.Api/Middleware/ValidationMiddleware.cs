using FluentValidation;
using System.Text.Json;

namespace Fiap.Game.Api.Middleware
{
    public class ValidationMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext context, IServiceProvider serviceProvider)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/problem+json";

                var errors = ex.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                var payload = new
                {
                    type = "https://httpstatuses.com/400",
                    title = "Validation Error",
                    status = StatusCodes.Status400BadRequest,
                    detail = "Um ou mais campos de validação falharam",
                    errors,
                    traceId = context.TraceIdentifier
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
        }
    }
}

