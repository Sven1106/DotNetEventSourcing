using System.Text.Json;
using KurrentDB.Client;

namespace KurrentExample;

public abstract record DomainEvent
{
    public static implicit operator EventData(DomainEvent @event)
    {
        return new EventData(
            Uuid.NewUuid(),
            @event.GetType().Name,
            JsonSerializer.SerializeToUtf8Bytes(@event)
        );
    }
}