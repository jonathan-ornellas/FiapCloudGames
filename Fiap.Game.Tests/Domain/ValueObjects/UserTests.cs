using Fiap.Game.Domain.Entities;
using Fiap.Game.Domain.Enum;
using Fiap.Game.Domain.ValueObjects;

namespace Fiap.Game.Tests.Domain.Entities
{
    public class UserTests
    {
        [Fact]
        public void User_ValidData_ShouldCreateSuccessfully()
        {
            // Arrange
            var name = "João Silva";
            var email = new Email("joao@exemplo.com");
            var hashedPassword = "hashedPassword123";
            var role = Role.User;

            // Act
            var user = new User(name, email, hashedPassword, role);

            // Assert
            user.Name.Should().Be(name);
            user.Email.Should().Be(email);
            user.Password.Should().Be(hashedPassword);
            user.Role.Should().Be(role);
        }

        [Fact]
        public void User_DefaultRole_ShouldBeUser()
        {
            // Arrange
            var name = "João Silva";
            var email = new Email("joao@exemplo.com");
            var hashedPassword = "hashedPassword123";

            // Act
            var user = new User(name, email, hashedPassword);

            // Assert
            user.Role.Should().Be(Role.User);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void User_SetName_EmptyOrNull_ShouldThrowArgumentException(string invalidName)
        {
            // Arrange
            var email = new Email("joao@exemplo.com");
            var hashedPassword = "hashedPassword123";

            // Act & Assert
            var action = () => new User(invalidName, email, hashedPassword);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Nome não pode ser vazio*");
        }

        [Fact]
        public void User_SetName_TooLong_ShouldThrowArgumentException()
        {
            // Arrange
            var longName = new string('a', 121); // Mais de 120 caracteres
            var email = new Email("joao@exemplo.com");
            var hashedPassword = "hashedPassword123";

            // Act & Assert
            var action = () => new User(longName, email, hashedPassword);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Nome não pode ter mais de 120 caracteres*");
        }

        [Fact]
        public void User_SetName_ShouldTrimWhitespace()
        {
            // Arrange
            var nameWithSpaces = "  João Silva  ";
            var email = new Email("joao@exemplo.com");
            var hashedPassword = "hashedPassword123";

            // Act
            var user = new User(nameWithSpaces, email, hashedPassword);

            // Assert
            user.Name.Should().Be("João Silva");
        }

        [Fact]
        public void User_SetEmail_Null_ShouldThrowArgumentNullException()
        {
            // Arrange
            var name = "João Silva";
            var hashedPassword = "hashedPassword123";

            // Act & Assert
            var action = () => new User(name, null!, hashedPassword);
            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void User_SetPassword_EmptyOrNull_ShouldThrowArgumentException(string invalidPassword)
        {
            // Arrange
            var name = "João Silva";
            var email = new Email("joao@exemplo.com");

            // Act & Assert
            var action = () => new User(name, email, invalidPassword);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Senha não pode ser vazia*");
        }

        [Fact]
        public void User_IsAdmin_WhenRoleIsAdmin_ShouldReturnTrue()
        {
            // Arrange
            var name = "Admin User";
            var email = new Email("admin@exemplo.com");
            var hashedPassword = "hashedPassword123";
            var user = new User(name, email, hashedPassword, Role.Admin);

            // Act
            var isAdmin = user.IsAdmin();

            // Assert
            isAdmin.Should().BeTrue();
        }

        [Fact]
        public void User_IsAdmin_WhenRoleIsUser_ShouldReturnFalse()
        {
            // Arrange
            var name = "Regular User";
            var email = new Email("user@exemplo.com");
            var hashedPassword = "hashedPassword123";
            var user = new User(name, email, hashedPassword, Role.User);

            // Act
            var isAdmin = user.IsAdmin();

            // Assert
            isAdmin.Should().BeFalse();
        }

        [Fact]
        public void User_SetRole_ShouldUpdateRole()
        {
            // Arrange
            var name = "João Silva";
            var email = new Email("joao@exemplo.com");
            var hashedPassword = "hashedPassword123";
            var user = new User(name, email, hashedPassword);

            // Act
            user.SetRole(Role.Admin);

            // Assert
            user.Role.Should().Be(Role.Admin);
        }

        [Fact]
        public void User_SetName_ShouldUpdateName()
        {
            // Arrange
            var name = "João Silva";
            var email = new Email("joao@exemplo.com");
            var hashedPassword = "hashedPassword123";
            var user = new User(name, email, hashedPassword);

            // Act
            user.SetName("João Santos");

            // Assert
            user.Name.Should().Be("João Santos");
        }

        [Fact]
        public void User_SetEmail_ShouldUpdateEmail()
        {
            // Arrange
            var name = "João Silva";
            var email = new Email("joao@exemplo.com");
            var hashedPassword = "hashedPassword123";
            var user = new User(name, email, hashedPassword);
            var newEmail = new Email("joao.novo@exemplo.com");

            // Act
            user.SetEmail(newEmail);

            // Assert
            user.Email.Should().Be(newEmail);
        }

        [Fact]
        public void User_SetPassword_ShouldUpdatePassword()
        {
            // Arrange
            var name = "João Silva";
            var email = new Email("joao@exemplo.com");
            var hashedPassword = "hashedPassword123";
            var user = new User(name, email, hashedPassword);
            var newHashedPassword = "newHashedPassword456";

            // Act
            user.SetPassword(newHashedPassword);

            // Assert
            user.Password.Should().Be(newHashedPassword);
        }
    }
}

