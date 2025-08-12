using Fiap.Game.Domain.Entities.Base;
using Fiap.Game.Domain.Enum;
using Fiap.Game.Domain.ValueObjects;

namespace Fiap.Game.Domain.Entities
{
    public class User : Entity
    {
        public string Name { get; private set; } = string.Empty;
        public Email Email { get; private set; } = null!;
        public string Password { get; private set; } = string.Empty;
        public Role Role { get; private set; } = Role.User;

        protected User() { }

        public User(string name, Email email, string hashedPassword, Role role = Role.User)
        {
            SetName(name);
            SetEmail(email);
            SetPassword(hashedPassword);
            SetRole(role);
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome não pode ser vazio", nameof(name));

            if (name.Length > 120)
                throw new ArgumentException("Nome não pode ter mais de 120 caracteres", nameof(name));

            Name = name.Trim();
        }

        public void SetEmail(Email email)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }

        public void SetPassword(string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
                throw new ArgumentException("Senha não pode ser vazia", nameof(hashedPassword));

            Password = hashedPassword;
        }

        public void SetRole(Role role)
        {
            Role = role;
        }

        public bool IsAdmin() => Role == Role.Admin;
    }
}
