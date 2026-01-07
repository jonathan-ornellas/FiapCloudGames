using FiapCloudGames.Domain.ValueObjects;
using Xunit;

namespace FiapCloudGames.Tests.Unit.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@example.com")]
    [InlineData("user+tag@example.co.uk")]
    [InlineData("test123@domain.org")]
    public void Constructor_ValidEmail_CreatesEmail(string email)
    {
        var emailValue = new Email(email);

        Assert.Equal(email.ToLowerInvariant(), emailValue.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_EmptyEmail_ThrowsArgumentException(string email)
    {
        var exception = Assert.Throws<ArgumentException>(() => new Email(email));

        Assert.Equal("Email não pode ser vazio (Parameter 'value')", exception.Message);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user@domain")]
    [InlineData("user domain@example.com")]
    public void Constructor_InvalidFormat_ThrowsArgumentException(string email)
    {
        var exception = Assert.Throws<ArgumentException>(() => new Email(email));

        Assert.Equal("Email deve ter um formato válido (Parameter 'value')", exception.Message);
    }

    [Fact]
    public void Constructor_TooLongEmail_ThrowsArgumentException()
    {
        var longEmail = new string('a', 170) + "@example.com";

        var exception = Assert.Throws<ArgumentException>(() => new Email(longEmail));

        Assert.Equal("Email não pode ter mais de 180 caracteres (Parameter 'value')", exception.Message);
    }

    [Theory]
    [InlineData("TEST@EXAMPLE.COM", "test@example.com")]
    [InlineData("User@Domain.COM", "user@domain.com")]
    [InlineData("  user@example.com  ", "user@example.com")]
    public void Constructor_NormalizesEmail(string input, string expected)
    {
        var email = new Email(input);

        Assert.Equal(expected, email.Value);
    }

    [Fact]
    public void IsValid_ValidEmail_ReturnsTrue()
    {
        Assert.True(Email.IsValid("test@example.com"));
    }

    [Fact]
    public void IsValid_InvalidEmail_ReturnsFalse()
    {
        Assert.False(Email.IsValid("invalid"));
    }

    [Fact]
    public void Equals_SameEmail_ReturnsTrue()
    {
        var email1 = new Email("test@example.com");
        var email2 = new Email("test@example.com");

        Assert.True(email1.Equals(email2));
        Assert.True(email1 == email2);
    }

    [Fact]
    public void Equals_DifferentEmail_ReturnsFalse()
    {
        var email1 = new Email("test1@example.com");
        var email2 = new Email("test2@example.com");

        Assert.False(email1.Equals(email2));
        Assert.True(email1 != email2);
    }

    [Fact]
    public void ImplicitConversion_ToString_Works()
    {
        var email = new Email("test@example.com");
        string emailString = email;

        Assert.Equal("test@example.com", emailString);
    }

    [Fact]
    public void ImplicitConversion_FromString_Works()
    {
        Email email = "test@example.com";

        Assert.Equal("test@example.com", email.Value);
    }

    [Fact]
    public void GetHashCode_SameEmail_ReturnsSameHashCode()
    {
        var email1 = new Email("test@example.com");
        var email2 = new Email("test@example.com");

        Assert.Equal(email1.GetHashCode(), email2.GetHashCode());
    }
}
