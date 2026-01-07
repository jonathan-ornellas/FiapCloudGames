using System.Text.RegularExpressions;

namespace FiapCloudGames.Domain.ValueObjects
{
    public class Email : IEquatable<Email>
    {
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        public string Value { get; private set; }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email não pode ser vazio", nameof(value));

            var normalizedEmail = value.Trim().ToLowerInvariant();

            if (!IsValid(normalizedEmail))
                throw new ArgumentException("Email deve ter um formato válido", nameof(value));

            if (normalizedEmail.Length > 180)
                throw new ArgumentException("Email não pode ter mais de 180 caracteres", nameof(value));

            Value = normalizedEmail;
        }

        public static bool IsValid(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);
        }

        public static implicit operator string(Email email) => email.Value;
        public static implicit operator Email(string value) => new(value);

        public override string ToString() => Value;
        
        public bool Equals(Email? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object? obj) => obj is Email other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(Email? left, Email? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Email? left, Email? right) => !(left == right);
    }
}
