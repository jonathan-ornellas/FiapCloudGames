using FiapCloudGames.Users.Api.DTOs;
using FiapCloudGames.Users.Api.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace FiapCloudGames.Tests.Unit.Validators;

public class RegisterUserDtoValidatorTests
{
    private readonly RegisterUserDtoValidator _validator;

    public RegisterUserDtoValidatorTests()
    {
        _validator = new RegisterUserDtoValidator();
    }

    [Fact]
    public void Validate_ValidDto_PassesValidation()
    {
        var dto = new RegisterUserDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "Test@123"
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
        var dto = new RegisterUserDto
        {
            Name = name,
            Email = "test@example.com",
            Password = "Test@123"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_InvalidEmail_FailsValidation(string email)
    {
        var dto = new RegisterUserDto
        {
            Name = "Test User",
            Email = email,
            Password = "Test@123"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("123")]
    [InlineData("short")]
    public void Validate_InvalidPassword_FailsValidation(string password)
    {
        var dto = new RegisterUserDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = password
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("Test@123")]
    [InlineData("Password@1")]
    [InlineData("MySecurePass123!")]
    public void Validate_ValidPassword_PassesValidation(string password)
    {
        var dto = new RegisterUserDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = password
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user@domain.co.uk")]
    [InlineData("name.surname@company.org")]
    public void Validate_ValidEmail_PassesValidation(string email)
    {
        var dto = new RegisterUserDto
        {
            Name = "Test User",
            Email = email,
            Password = "Test@123"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_AllFieldsEmpty_FailsValidation()
    {
        var dto = new RegisterUserDto
        {
            Name = "",
            Email = "",
            Password = ""
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
