namespace FiapCloudGames.EventSourcing;

using FiapCloudGames.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

public class EventStore
{
    public Guid Id { get; set; }
    public Guid AggregateId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string EventData { get; set; } = string.Empty;
    public int Version { get; set; }
    public DateTime Timestamp { get; set; }
}

public class EventSourcingContext : DbContext
{
    public DbSet<EventStore> Events { get; set; }

    public EventSourcingContext(DbContextOptions<EventSourcingContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventStore>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<EventStore>()
            .HasIndex(e => e.AggregateId);

        modelBuilder.Entity<EventStore>()
            .HasIndex(e => new { e.AggregateId, e.Version })
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}

public interface IEventStore
{
    Task AppendEventAsync(DomainEvent @event);
    Task<IEnumerable<DomainEvent>> GetEventsAsync(Guid aggregateId);
    Task<IEnumerable<DomainEvent>> GetEventsByTypeAsync(string eventType);
}

public class EventStoreRepository : IEventStore
{
    private readonly EventSourcingContext _context;

    public EventStoreRepository(EventSourcingContext context)
    {
        _context = context;
    }

    public async Task AppendEventAsync(DomainEvent @event)
    {
        var eventStore = new EventStore
        {
            Id = Guid.NewGuid(),
            AggregateId = @event.AggregateId,
            EventType = @event.GetType().Name,
            EventData = JsonConvert.SerializeObject(@event),
            Version = @event.Version,
            Timestamp = @event.Timestamp
        };

        _context.Events.Add(eventStore);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<DomainEvent>> GetEventsAsync(Guid aggregateId)
    {
        var events = await _context.Events
            .Where(e => e.AggregateId == aggregateId)
            .OrderBy(e => e.Version)
            .ToListAsync();

        return events.Select(DeserializeEvent).ToList();
    }

    public async Task<IEnumerable<DomainEvent>> GetEventsByTypeAsync(string eventType)
    {
        var events = await _context.Events
            .Where(e => e.EventType == eventType)
            .OrderBy(e => e.Timestamp)
            .ToListAsync();

        return events.Select(DeserializeEvent).ToList();
    }

    private DomainEvent DeserializeEvent(EventStore eventStore)
    {
        var type = Type.GetType($"FiapCloudGames.Shared.Models.{eventStore.EventType}");
        if (type == null)
            throw new InvalidOperationException($"Event type {eventStore.EventType} not found");

        return JsonConvert.DeserializeObject(eventStore.EventData, type) as DomainEvent
            ?? throw new InvalidOperationException($"Failed to deserialize event {eventStore.EventType}");
    }
}
