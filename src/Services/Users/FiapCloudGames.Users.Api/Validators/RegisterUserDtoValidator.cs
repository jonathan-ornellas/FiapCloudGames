using FluentValidation;
using FiapCloudGames.Users.Api.DTOs;


namespace FiapCloudGames.Users.Api.Validators;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("A valid email is required.");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain one or more capital letters.")
            .Matches("[a-z]").WithMessage("Password must contain one or more lowercase letters.")
            .Matches("[0-9]").WithMessage("Password must contain one or more numbers.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain one or more special characters.");
    }
}
