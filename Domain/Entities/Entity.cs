using Domain.Events;

namespace Domain.Entities;

public abstract class Entity
{
    public Guid Id { get; }

    private readonly List<Event> _events = [];

    public IReadOnlyCollection<Event> Events => _events.AsReadOnly();
    
    protected Entity(Guid id)
    {
        Id = id;
    }

    protected void AddEvent(Event @event)
    {
        _events.Add(@event);
    }

    public void ClearDomainEvents()
    {
        _events.Clear();
    }
}