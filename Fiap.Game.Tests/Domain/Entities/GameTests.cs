namespace Fiap.Game.Tests.Domain.Entities
{
    public class GameTests
    {
        [Fact]
        public void Game_ValidData_ShouldCreateSuccessfully()
        {
            // Arrange
            var title = "Super Game";
            var description = "Um jogo incrível";
            var price = 59.99m;
            var releaseDate = DateTime.UtcNow.AddDays(30);

            // Act
            var game = new Fiap.Game.Domain.Entities.Game(title, description, price, releaseDate);

            // Assert
            game.Title.Should().Be(title);
            game.Description.Should().Be(description);
            game.Price.Should().Be(price);
            game.ReleaseDate.Should().Be(releaseDate);
            game.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Game_WithoutReleaseDate_ShouldCreateSuccessfully()
        {
            // Arrange
            var title = "Super Game";
            var description = "Um jogo incrível";
            var price = 59.99m;

            // Act
            var game = new Fiap.Game.Domain.Entities.Game(title, description, price);

            // Assert
            game.Title.Should().Be(title);
            game.Description.Should().Be(description);
            game.Price.Should().Be(price);
            game.ReleaseDate.Should().BeNull();
            game.IsActive.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Game_SetTitle_EmptyOrNull_ShouldThrowArgumentException(string invalidTitle)
        {
            // Act & Assert
            var action = () => new Fiap.Game.Domain.Entities.Game(invalidTitle, "Descrição", 10.0m);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Título não pode ser vazio*");
        }

        [Fact]
        public void Game_SetTitle_TooLong_ShouldThrowArgumentException()
        {
            // Arrange
            var longTitle = new string('a', 201); // Mais de 200 caracteres

            // Act & Assert
            var action = () => new Fiap.Game.Domain.Entities.Game(longTitle, "Descrição", 10.0m);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Título não pode ter mais de 200 caracteres*");
        }

        [Fact]
        public void Game_SetTitle_ShouldTrimWhitespace()
        {
            // Arrange
            var titleWithSpaces = "  Super Game  ";

            // Act
            var game = new Fiap.Game.Domain.Entities.Game(titleWithSpaces, "Descrição", 10.0m);

            // Assert
            game.Title.Should().Be("Super Game");
        }

        [Fact]
        public void Game_SetDescription_TooLong_ShouldThrowArgumentException()
        {
            // Arrange
            var longDescription = new string('a', 1001); // Mais de 1000 caracteres

            // Act & Assert
            var action = () => new Fiap.Game.Domain.Entities.Game("Título", longDescription, 10.0m);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Descrição não pode ter mais de 1000 caracteres*");
        }

        [Fact]
        public void Game_SetDescription_ShouldTrimWhitespace()
        {
            // Arrange
            var descriptionWithSpaces = "  Uma descrição incrível  ";

            // Act
            var game = new Fiap.Game.Domain.Entities.Game("Título", descriptionWithSpaces, 10.0m);

            // Assert
            game.Description.Should().Be("Uma descrição incrível");
        }

        [Fact]
        public void Game_SetDescription_Null_ShouldAccept()
        {
            // Act
            var game = new Fiap.Game.Domain.Entities.Game("Título", null, 10.0m);

            // Assert
            game.Description.Should().BeNull();
        }

        [Fact]
        public void Game_SetPrice_Negative_ShouldThrowArgumentException()
        {
            // Act & Assert
            var action = () => new Fiap.Game.Domain.Entities.Game("Título", "Descrição", -10.0m);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Preço não pode ser negativo*");
        }

        [Fact]
        public void Game_SetPrice_Zero_ShouldAccept()
        {
            // Act
            var game = new Fiap.Game.Domain.Entities.Game("Título", "Descrição", 0m);

            // Assert
            game.Price.Should().Be(0m);
        }

        [Fact]
        public void Game_Activate_ShouldSetIsActiveToTrue()
        {
            // Arrange
            var game = new Fiap.Game.Domain.Entities.Game("Título", "Descrição", 10.0m);
            game.Deactivate(); // Primeiro desativa

            // Act
            game.Activate();

            // Assert
            game.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Game_Deactivate_ShouldSetIsActiveToFalse()
        {
            // Arrange
            var game = new Fiap.Game.Domain.Entities.Game("Título", "Descrição", 10.0m);

            // Act
            game.Deactivate();

            // Assert
            game.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Game_IsReleased_WhenReleaseDateIsInPast_ShouldReturnTrue()
        {
            // Arrange
            var pastDate = DateTime.UtcNow.AddDays(-1);
            var game = new Fiap.Game.Domain.Entities.Game("Título", "Descrição", 10.0m, pastDate);

            // Act
            var isReleased = game.IsReleased();

            // Assert
            isReleased.Should().BeTrue();
        }

        [Fact]
        public void Game_IsReleased_WhenReleaseDateIsInFuture_ShouldReturnFalse()
        {
            // Arrange
            var futureDate = DateTime.UtcNow.AddDays(1);
            var game = new Fiap.Game.Domain.Entities.Game("Título", "Descrição", 10.0m, futureDate);

            // Act
            var isReleased = game.IsReleased();

            // Assert
            isReleased.Should().BeFalse();
        }

        [Fact]
        public void Game_IsReleased_WhenReleaseDateIsNull_ShouldReturnFalse()
        {
            // Arrange
            var game = new Fiap.Game.Domain.Entities.Game("Título", "Descrição", 10.0m);

            // Act
            var isReleased = game.IsReleased();

            // Assert
            isReleased.Should().BeFalse();
        }

        [Fact]
        public void Game_IsFree_WhenPriceIsZero_ShouldReturnTrue()
        {
            // Arrange
            var game = new Fiap.Game.Domain.Entities.Game("Título", "Descrição", 0m);

            // Act
            var isFree = game.IsFree();

            // Assert
            isFree.Should().BeTrue();
        }

        [Fact]
        public void Game_IsFree_WhenPriceIsGreaterThanZero_ShouldReturnFalse()
        {
            // Arrange
            var game = new Fiap.Game.Domain.Entities.Game("Título", "Descrição", 10.0m);

            // Act
            var isFree = game.IsFree();

            // Assert
            isFree.Should().BeFalse();
        }

        [Fact]
        public void Game_SetTitle_ShouldUpdateTitle()
        {
            // Arrange
            var game = new Fiap.Game.Domain.Entities.Game("Título Original", "Descrição", 10.0m);

            // Act
            game.SetTitle("Novo Título");

            // Assert
            game.Title.Should().Be("Novo Título");
        }

        [Fact]
        public void Game_SetDescription_ShouldUpdateDescription()
        {
            // Arrange
            var game = new Fiap.Game.Domain.Entities.Game("Título", "Descrição Original", 10.0m);

            // Act
            game.SetDescription("Nova Descrição");

            // Assert
            game.Description.Should().Be("Nova Descrição");
        }

        [Fact]
        public void Game_SetPrice_ShouldUpdatePrice()
        {
            // Arrange
            var game = new Fiap.Game.Domain.Entities.Game("Título", "Descrição", 10.0m);

            // Act
            game.SetPrice(20.0m);

            // Assert
            game.Price.Should().Be(20.0m);
        }

        [Fact]
        public void Game_SetReleaseDate_ShouldUpdateReleaseDate()
        {
            // Arrange
            var game = new Fiap.Game.Domain.Entities.Game("Título", "Descrição", 10.0m);
            var newReleaseDate = DateTime.UtcNow.AddDays(15);

            // Act
            game.SetReleaseDate(newReleaseDate);

            // Assert
            game.ReleaseDate.Should().Be(newReleaseDate);
        }
    }
}

