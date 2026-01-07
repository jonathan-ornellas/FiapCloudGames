using Fiap.Game.Domain.Entities.Base;

namespace Fiap.Game.Domain.Entities
{
    public class Game : Entity
    {
        public string Title { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public DateTime? ReleaseDate { get; private set; }
        public bool IsActive { get; private set; } = true;

        protected Game() { }

        public Game(string title, string? description, decimal price, DateTime? releaseDate = null)
        {
            SetTitle(title);
            SetDescription(description);
            SetPrice(price);
            SetReleaseDate(releaseDate);
        }

        public void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Título não pode ser vazio", nameof(title));

            if (title.Length > 200)
                throw new ArgumentException("Título não pode ter mais de 200 caracteres", nameof(title));

            Title = title.Trim();
        }

        public void SetDescription(string? description)
        {
            if (!string.IsNullOrWhiteSpace(description) && description.Length > 1000)
                throw new ArgumentException("Descrição não pode ter mais de 1000 caracteres", nameof(description));

            Description = description?.Trim();
        }

        public void SetPrice(decimal price)
        {
            if (price < 0)
                throw new ArgumentException("Preço não pode ser negativo", nameof(price));

            Price = price;
        }

        public void SetReleaseDate(DateTime? releaseDate)
        {
            ReleaseDate = releaseDate;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public bool IsReleased() => ReleaseDate.HasValue && ReleaseDate.Value <= DateTime.UtcNow;
        public bool IsFree() => Price == 0;
    }
}
