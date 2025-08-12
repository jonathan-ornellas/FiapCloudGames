using FluentValidation;

namespace Fiap.Game.Api.Validation
{
    public class ValidationFilter<T> : IEndpointFilter where T : class
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
        {
            var validator = ctx.HttpContext.RequestServices.GetService<IValidator<T>>();
            var arg = ctx.Arguments.FirstOrDefault(a => a is T) as T;

            if (validator is not null && arg is not null)
            {
                var result = await validator.ValidateAsync(arg);
                if (!result.IsValid)
                {
                    var errors = result.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                    return Results.ValidationProblem(errors);
                }
            }

            return await next(ctx);
        }
    }
}
