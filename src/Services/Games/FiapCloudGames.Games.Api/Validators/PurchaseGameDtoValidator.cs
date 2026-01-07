using FluentValidation;
using FiapCloudGames.Games.Api.DTOs;

namespace FiapCloudGames.Games.Api.Validators;

public class PurchaseGameDtoValidator : AbstractValidator<PurchaseGameDto>
{
    public PurchaseGameDtoValidator()
    {
        RuleFor(x => x.GameId)
            .GreaterThan(0)
            .WithMessage("Game ID must be greater than zero.");

        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than zero.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .WithMessage("Payment method is required.");
    }
}
