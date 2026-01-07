using FiapCloudGames.Domain.ValueObjects;

namespace FiapCloudGames.Domain.Entities
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public Money Price { get; set; }
        public double Rating { get; set; }

        public Game(string title, string description, string genre, Money price, double rating)
        {
            Title = title;
            Description = description;
            Genre = genre;
            Price = price;
            Rating = rating;
        }
    }
}
