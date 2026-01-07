using Fiap.Game.Domain.Entities.Base;

namespace Fiap.Game.Domain.Entities
{
    public class LibraryItem  : Entity
    {
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public decimal PricePaid { get; set; }
    }
    
}
