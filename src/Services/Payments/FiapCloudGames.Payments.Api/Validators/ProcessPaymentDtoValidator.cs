using FluentValidation;
using FiapCloudGames.Payments.Api.DTOs;


namespace FiapCloudGames.Payments.Api.Validators;

public class ProcessPaymentDtoValidator : AbstractValidator<ProcessPaymentDto>
{
    public ProcessPaymentDtoValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
        RuleFor(x => x.GameId).NotEmpty().WithMessage("Game ID is required.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
        RuleFor(x => x.PaymentMethod).NotEmpty().WithMessage("Payment method is required.");
    }
}
