using FluentValidation;
using FiapCloudGames.Games.Api.DTOs;

namespace FiapCloudGames.Games.Api.Validators;

public class CreateGameDtoValidator : AbstractValidator<CreateGameDto>
{
    public CreateGameDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
        RuleFor(x => x.Category).NotEmpty().WithMessage("Category is required.");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
        RuleFor(x => x.Rating).InclusiveBetween(0, 10).WithMessage("Rating must be between 0 and 10.");
    }
}
