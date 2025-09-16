using System.Text.Json;
using KurrentDB.Client;

namespace KurrentExample;

public abstract record DomainEvent
{
    public static implicit operator EventData(DomainEvent @event)
    {
        var type = @event.GetType();
        var data = JsonSerializer.SerializeToUtf8Bytes(@event, type, JsonDefaults.Options);

        return new EventData(
            Uuid.NewUuid(),
            type.Name,
            data
        );
    }
}