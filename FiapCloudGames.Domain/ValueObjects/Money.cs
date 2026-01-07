namespace FiapCloudGames.Domain.ValueObjects
{
    public class Money : IEquatable<Money>
    {
        public decimal Value { get; private set; }

        public Money(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Valor monetário não pode ser negativo", nameof(value));

            Value = value;
        }

        public static implicit operator decimal(Money money) => money.Value;
        public static implicit operator Money(decimal value) => new(value);

        public override string ToString() => Value.ToString("C");

        public bool Equals(Money? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object? obj) => obj is Money other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(Money? left, Money? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Money? left, Money? right) => !(left == right);
    }
}
