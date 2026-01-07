using System.Text.RegularExpressions;

namespace Fiap.Game.Domain.ValueObjects
{
    public class Password
    {
        private static readonly Regex PasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$");

        public string Value { get; private set; }

        public Password(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Senha não pode ser vazia", nameof(value));

            if (!IsValid(value))
                throw new ArgumentException("Senha deve ter pelo menos 8 caracteres, incluindo letras maiúsculas, minúsculas, números e caracteres especiais", nameof(value));

            Value = value;
        }

        public static bool IsValid(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && PasswordRegex.IsMatch(password);
        }

        public static implicit operator string(Password password) => password.Value;
        public static implicit operator Password(string value) => new(value);

        public override string ToString() => Value;
        public override bool Equals(object? obj) => obj is Password other && Value == other.Value;
        public override int GetHashCode() => Value.GetHashCode();
    }
}

