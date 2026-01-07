using FiapCloudGames.Domain.ValueObjects;

namespace FiapCloudGames.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Email Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public User(string name, Email email, string password, string role = "User")
        {
            Name = name;
            Email = email;
            Password = password;
            Role = role;
        }
    }
}
