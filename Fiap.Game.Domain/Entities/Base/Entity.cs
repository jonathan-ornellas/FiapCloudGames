namespace Fiap.Game.Domain.Entities.Base
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }

        protected Entity()
        {
            if (Id == default) Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }

        public void Touch() => UpdatedAt = DateTime.Now;

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not Entity other) return false;
            if (GetType() != other.GetType()) return false;
            if (Id == default || other.Id == default) return false;
            return Id == other.Id;
        }

        public override int GetHashCode() => HashCode.Combine(GetType(), Id);
        public static bool operator ==(Entity? a, Entity? b) => a?.Equals(b) ?? b is null;
        public static bool operator !=(Entity? a, Entity? b) => !(a == b);
    }
}
