using FiapCloudGames.Users.Api.DTOs;
using FiapCloudGames.Users.Api.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace FiapCloudGames.Tests.Unit.Validators;

public class UpdateProfileDtoValidatorTests
{
    private readonly UpdateProfileDtoValidator _validator;

    public UpdateProfileDtoValidatorTests()
    {
        _validator = new UpdateProfileDtoValidator();
    }

    [Fact]
    public void Validate_ValidDto_PassesValidation()
    {
        var dto = new UpdateProfileDto
        {
            Name = "João Silva"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_EmptyName_FailsValidation(string name)
    {
        var dto = new UpdateProfileDto
        {
            Name = name
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("Jo")]
    [InlineData("Ab")]
    public void Validate_ShortName_FailsValidation(string name)
    {
        var dto = new UpdateProfileDto
        {
            Name = name
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("João")]
    [InlineData("Maria Silva")]
    [InlineData("Pedro Henrique Santos")]
    public void Validate_ValidName_PassesValidation(string name)
    {
        var dto = new UpdateProfileDto
        {
            Name = name
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
