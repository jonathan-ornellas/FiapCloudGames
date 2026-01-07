using FiapCloudGames.Games.Api.DTOs;
using FiapCloudGames.Games.Api.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace FiapCloudGames.Tests.Unit.Validators;

public class PurchaseGameDtoValidatorTests
{
    private readonly PurchaseGameDtoValidator _validator;

    public PurchaseGameDtoValidatorTests()
    {
        _validator = new PurchaseGameDtoValidator();
    }

    [Fact]
    public void Validate_ValidDto_PassesValidation()
    {
        var dto = new PurchaseGameDto
        {
            GameId = 1,
            UserId = 1,
            Amount = 59.99m,
            PaymentMethod = "CreditCard"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Validate_InvalidGameId_FailsValidation(int gameId)
    {
        var dto = new PurchaseGameDto
        {
            GameId = gameId,
            UserId = 1,
            Amount = 59.99m,
            PaymentMethod = "CreditCard"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.GameId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Validate_InvalidUserId_FailsValidation(int userId)
    {
        var dto = new PurchaseGameDto
        {
            GameId = 1,
            UserId = userId,
            Amount = 59.99m,
            PaymentMethod = "CreditCard"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.50)]
    public void Validate_InvalidAmount_FailsValidation(decimal amount)
    {
        var dto = new PurchaseGameDto
        {
            GameId = 1,
            UserId = 1,
            Amount = amount,
            PaymentMethod = "CreditCard"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_EmptyPaymentMethod_FailsValidation(string paymentMethod)
    {
        var dto = new PurchaseGameDto
        {
            GameId = 1,
            UserId = 1,
            Amount = 59.99m,
            PaymentMethod = paymentMethod
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.PaymentMethod);
    }

    [Theory]
    [InlineData("CreditCard")]
    [InlineData("DebitCard")]
    [InlineData("Pix")]
    [InlineData("Boleto")]
    public void Validate_ValidPaymentMethod_PassesValidation(string paymentMethod)
    {
        var dto = new PurchaseGameDto
        {
            GameId = 1,
            UserId = 1,
            Amount = 59.99m,
            PaymentMethod = paymentMethod
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.PaymentMethod);
    }

    [Fact]
    public void Validate_AllFieldsInvalid_FailsValidation()
    {
        var dto = new PurchaseGameDto
        {
            GameId = 0,
            UserId = 0,
            Amount = 0,
            PaymentMethod = ""
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.GameId);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
        result.ShouldHaveValidationErrorFor(x => x.PaymentMethod);
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(10.50)]
    [InlineData(99.99)]
    [InlineData(999.99)]
    public void Validate_ValidAmount_PassesValidation(decimal amount)
    {
        var dto = new PurchaseGameDto
        {
            GameId = 1,
            UserId = 1,
            Amount = amount,
            PaymentMethod = "CreditCard"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.Amount);
    }
}
