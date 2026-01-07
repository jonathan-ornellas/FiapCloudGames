using Fiap.Game.Domain.ValueObjects;

namespace Fiap.Game.Tests.Domain.ValueObjects
{
    public class PasswordTests
    {
        [Fact]
        public void Password_ValidPassword_ShouldCreateSuccessfully()
        {
            var validPassword = "MinhaSenh@123";

            var password = new Password(validPassword);

            password.Value.Should().Be(validPassword);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Password_EmptyOrNullPassword_ShouldThrowArgumentException(string invalidPassword)
        {
            var action = () => new Password(invalidPassword);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Senha não pode ser vazia*");
        }

        [Theory]
        [InlineData("123456")]
        [InlineData("abcdef")]
        [InlineData("ABCDEF")]
        [InlineData("Abc123")]
        [InlineData("Abc@12")]
        public void Password_InvalidPassword_ShouldThrowArgumentException(string invalidPassword)
        {
            var action = () => new Password(invalidPassword);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Senha deve ter pelo menos 8 caracteres*");
        }

        [Theory]
        [InlineData("MinhaSenh@123")]
        [InlineData("OutraSenhateste#456")]
        [InlineData("SenhaForte$789")]
        public void Password_ValidPasswords_ShouldPassValidation(string validPassword)
        {
            var isValid = Password.IsValid(validPassword);

            isValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("123456")]
        [InlineData("abcdef")]
        [InlineData("ABCDEF")]
        [InlineData("Abc123")]
        [InlineData("Abc@12")]
        public void Password_InvalidPasswords_ShouldFailValidation(string invalidPassword)
        {
            var isValid = Password.IsValid(invalidPassword);

            isValid.Should().BeFalse();
        }

        [Fact]
        public void Password_ImplicitConversionToString_ShouldWork()
        {
            var passwordValue = "MinhaSenh@123";
            var password = new Password(passwordValue);

            string result = password;

            result.Should().Be(passwordValue);
        }

        [Fact]
        public void Password_ImplicitConversionFromString_ShouldWork()
        {
            var passwordValue = "MinhaSenh@123";

            Password password = passwordValue;

            password.Value.Should().Be(passwordValue);
        }

        [Fact]
        public void Password_ToString_ShouldReturnValue()
        {
            var passwordValue = "MinhaSenh@123";
            var password = new Password(passwordValue);

            var result = password.ToString();

            result.Should().Be(passwordValue);
        }

        [Fact]
        public void Password_Equals_ShouldCompareValues()
        {
            var passwordValue = "MinhaSenh@123";
            var password1 = new Password(passwordValue);
            var password2 = new Password(passwordValue);

            password1.Equals(password2).Should().BeTrue();
            password1.GetHashCode().Should().Be(password2.GetHashCode());
        }
    }
}

