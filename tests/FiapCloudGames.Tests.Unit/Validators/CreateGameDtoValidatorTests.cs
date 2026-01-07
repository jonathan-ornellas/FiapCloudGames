using FiapCloudGames.Games.Api.DTOs;
using FiapCloudGames.Games.Api.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace FiapCloudGames.Tests.Unit.Validators;

public class CreateGameDtoValidatorTests
{
    private readonly CreateGameDtoValidator _validator;

    public CreateGameDtoValidatorTests()
    {
        _validator = new CreateGameDtoValidator();
    }

    [Fact]
    public void Validate_ValidDto_PassesValidation()
    {
        var dto = new CreateGameDto
        {
            Title = "Test Game",
            Description = "A test game description",
            Category = "Action",
            Price = 59.99m,
            Rating = 8
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_EmptyTitle_FailsValidation(string title)
    {
        var dto = new CreateGameDto
        {
            Title = title,
            Description = "A test game",
            Category = "Action",
            Price = 59.99m,
            Rating = 8
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_EmptyDescription_FailsValidation(string description)
    {
        var dto = new CreateGameDto
        {
            Title = "Test Game",
            Description = description,
            Category = "Action",
            Price = 59.99m,
            Rating = 8
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_EmptyCategory_FailsValidation(string category)
    {
        var dto = new CreateGameDto
        {
            Title = "Test Game",
            Description = "A test game",
            Category = category,
            Price = 59.99m,
            Rating = 8
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Category);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.50)]
    public void Validate_InvalidPrice_FailsValidation(decimal price)
    {
        var dto = new CreateGameDto
        {
            Title = "Test Game",
            Description = "A test game",
            Category = "Action",
            Price = price,
            Rating = 8
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    [InlineData(100)]
    public void Validate_InvalidRating_FailsValidation(int rating)
    {
        var dto = new CreateGameDto
        {
            Title = "Test Game",
            Description = "A test game",
            Category = "Action",
            Price = 59.99m,
            Rating = rating
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Rating);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(10)]
    public void Validate_ValidRating_PassesValidation(int rating)
    {
        var dto = new CreateGameDto
        {
            Title = "Test Game",
            Description = "A test game",
            Category = "Action",
            Price = 59.99m,
            Rating = rating
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.Rating);
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(1)]
    [InlineData(59.99)]
    [InlineData(9999.99)]
    public void Validate_ValidPrice_PassesValidation(decimal price)
    {
        var dto = new CreateGameDto
        {
            Title = "Test Game",
            Description = "A test game",
            Category = "Action",
            Price = price,
            Rating = 8
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_AllFieldsInvalid_FailsValidation()
    {
        var dto = new CreateGameDto
        {
            Title = "",
            Description = "",
            Category = "",
            Price = 0,
            Rating = -1
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title);
        result.ShouldHaveValidationErrorFor(x => x.Description);
        result.ShouldHaveValidationErrorFor(x => x.Category);
        result.ShouldHaveValidationErrorFor(x => x.Price);
        result.ShouldHaveValidationErrorFor(x => x.Rating);
    }
}
