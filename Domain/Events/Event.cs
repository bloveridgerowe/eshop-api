using MediatR;

namespace Domain.Events;

public abstract class Event : INotification
{
    public Guid EventId { get; }
    public DateTimeOffset OccurredOn { get; }

    protected Event()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTimeOffset.UtcNow;
    }
}