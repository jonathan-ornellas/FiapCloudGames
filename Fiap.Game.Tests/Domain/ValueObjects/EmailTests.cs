using Fiap.Game.Domain.ValueObjects;

namespace Fiap.Game.Tests.Domain.ValueObjects
{
    public class EmailTests
    {
        [Fact]
        public void Email_ValidEmail_ShouldCreateSuccessfully()
        {
            // Arrange
            var validEmail = "usuario@exemplo.com";

            // Act
            var email = new Email(validEmail);

            // Assert
            email.Value.Should().Be(validEmail.ToLowerInvariant());
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Email_EmptyOrNullEmail_ShouldThrowArgumentException(string invalidEmail)
        {
            // Act & Assert
            var action = () => new Email(invalidEmail);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Email não pode ser vazio*");
        }

        [Theory]
        [InlineData("email-sem-arroba")]
        [InlineData("@exemplo.com")]
        [InlineData("usuario@")]
        [InlineData("usuario@.com")]
        [InlineData("usuario.exemplo.com")]
        public void Email_InvalidFormat_ShouldThrowArgumentException(string invalidEmail)
        {
            // Act & Assert
            var action = () => new Email(invalidEmail);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Email deve ter um formato válido*");
        }

        [Fact]
        public void Email_TooLong_ShouldThrowArgumentException()
        {
            // Arrange
            var longEmail = new string('a', 170) + "@exemplo.com"; // Mais de 180 caracteres

            // Act & Assert
            var action = () => new Email(longEmail);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Email não pode ter mais de 180 caracteres*");
        }

        [Theory]
        [InlineData("usuario@exemplo.com")]
        [InlineData("teste.email@dominio.com.br")]
        [InlineData("user123@test-domain.org")]
        public void Email_ValidEmails_ShouldPassValidation(string validEmail)
        {
            // Act
            var isValid = Email.IsValid(validEmail);

            // Assert
            isValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("email-sem-arroba")]
        [InlineData("@exemplo.com")]
        [InlineData("usuario@")]
        [InlineData("usuario@.com")]
        public void Email_InvalidEmails_ShouldFailValidation(string invalidEmail)
        {
            // Act
            var isValid = Email.IsValid(invalidEmail);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void Email_ShouldNormalizeToLowerCase()
        {
            // Arrange
            var emailWithUpperCase = "USUARIO@EXEMPLO.COM";

            // Act
            var email = new Email(emailWithUpperCase);

            // Assert
            email.Value.Should().Be("usuario@exemplo.com");
        }

        [Fact]
        public void Email_ShouldTrimWhitespace()
        {
            // Arrange
            var emailWithSpaces = "  usuario@exemplo.com  ";

            // Act
            var email = new Email(emailWithSpaces);

            // Assert
            email.Value.Should().Be("usuario@exemplo.com");
        }

        [Fact]
        public void Email_ImplicitConversionToString_ShouldWork()
        {
            // Arrange
            var emailValue = "usuario@exemplo.com";
            var email = new Email(emailValue);

            // Act
            string result = email;

            // Assert
            result.Should().Be(emailValue);
        }

        [Fact]
        public void Email_ImplicitConversionFromString_ShouldWork()
        {
            // Arrange
            var emailValue = "usuario@exemplo.com";

            // Act
            Email email = emailValue;

            // Assert
            email.Value.Should().Be(emailValue);
        }

        [Fact]
        public void Email_ToString_ShouldReturnValue()
        {
            // Arrange
            var emailValue = "usuario@exemplo.com";
            var email = new Email(emailValue);

            // Act
            var result = email.ToString();

            // Assert
            result.Should().Be(emailValue);
        }

        [Fact]
        public void Email_Equals_ShouldCompareValues()
        {
            // Arrange
            var emailValue = "usuario@exemplo.com";
            var email1 = new Email(emailValue);
            var email2 = new Email(emailValue);

            // Act & Assert
            email1.Equals(email2).Should().BeTrue();
            email1.GetHashCode().Should().Be(email2.GetHashCode());
        }
    }
}

