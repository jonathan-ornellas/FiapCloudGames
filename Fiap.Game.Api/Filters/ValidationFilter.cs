using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fiap.Game.Api.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var parameter in context.ActionDescriptor.Parameters)
            {
                var argument = context.ActionArguments[parameter.Name];
                if (argument == null) continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(parameter.ParameterType);
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    var validationResult = await validator.ValidateAsync(new ValidationContext<object>(argument));
                    if (!validationResult.IsValid)
                    {
                        var errors = validationResult.Errors.GroupBy(e => e.PropertyName)
                            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                        var problemDetails = new ValidationProblemDetails(errors)
                        {
                            Type = "https://httpstatuses.com/400",
                            Title = "Validation Error",
                            Status = StatusCodes.Status400BadRequest,
                            Detail = "Um ou mais campos de validação falharam"
                        };

                        context.Result = new BadRequestObjectResult(problemDetails);
                        return;
                    }
                }
            }

            await next();
        }
    }
}

