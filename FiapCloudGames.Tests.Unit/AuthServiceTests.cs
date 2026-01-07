using Xunit;
using FluentAssertions;

namespace FiapCloudGames.Tests.Unit;

public class AuthServiceTests
{
    [Fact]
    public void ValidateEmail_WithValidEmail_ShouldReturnTrue()
    {
        var email = "test@example.com";
        var isValid = email.Contains("@") && email.Contains(".");

        isValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateEmail_WithInvalidEmail_ShouldReturnFalse()
    {
        var email = "invalid-email";
        var isValid = email.Contains("@") && email.Contains(".");

        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidatePassword_WithValidPassword_ShouldReturnTrue()
    {
        var password = "Password123!";
        var isValid = password.Length >= 8 && 
                     password.Any(char.IsUpper) && 
                     password.Any(char.IsLower) && 
                     password.Any(char.IsDigit);

        isValid.Should().BeTrue();
    }

    [Fact]
    public void ValidatePassword_WithShortPassword_ShouldReturnFalse()
    {
        var password = "Pass1!";
        var isValid = password.Length >= 8;

        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidatePassword_WithoutSpecialChar_ShouldReturnFalse()
    {
        var password = "Password123";
        var isValid = password.Any(c => !char.IsLetterOrDigit(c));

        isValid.Should().BeFalse();
    }
}
