using System.Text.Json;
using KurrentDB.Client;
using MartenExample.Endpoints;

namespace KurrentExample.Endpoints;

// 📥 Request models
public record AddItemRequest(string Name, string Description, List<ItemTag> Tags, uint Quantity);

public enum ItemTag
{
    Laptop,
    Desktop,
    Workstation,
    Monitor,
    Keyboard,
    Mouse,
}

public abstract class ItemEndpoints : IEndpoint
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/item", async (AddItemRequest request, KurrentDBClient client, CancellationToken ct) =>
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return Results.BadRequest("Name is required.");

                var itemId = Guid.NewGuid();
                await client.AppendToStreamAsync(
                    $"item-{itemId}",
                    StreamState.NoStream,
                    [
                        new ItemAdded(itemId, request.Name, request.Description, request.Tags, request.Quantity)
                    ],
                    cancellationToken: ct
                );

                return Results.Created($"/item/{itemId}", new { itemId });
            })
            .WithName("AddItem")
            .WithDescription("Adds a new item item with the given name, description, and quantity.")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        endpoints.MapGet("/item", async (KurrentDBClient client, CancellationToken ct) =>
            {
                var summaries = new Dictionary<Guid, ItemAggregate>();

                var result = client.ReadStreamAsync(
                    Direction.Forwards,
                    "$ce-item",
                    StreamPosition.Start,
                    cancellationToken: ct
                );

                await foreach (var resolved in result)
                {
                    var type = resolved.Event.EventType;
                    var data = resolved.Event.Data.Span;
                }

                return Results.Ok(summaries.Values);
            })
            .WithName("ListItems")
            .WithDescription("Returns a list of all item items for overview.")
            .Produces<List<ItemSummary>>();
    }
}

// 📦 Event
public record ItemAdded(Guid ItemId, string Name, string Description, List<ItemTag> Tags, uint Quantity) : DomainEvent;

// Aggregates

public record ItemAggregate(Guid Id, string Name, uint Quantity)
{
    public static ItemAggregate Create(ItemAdded e) => new(
        e.ItemId,
        e.Name,
        e.Quantity
    );
}

// Projections

#region ItemDetails

public record ItemDetails(Guid Id, string Name, string Description, List<ItemTag> Tags, uint Quantity);

public class ItemDetailsProjection
{
    public static ItemDetails Create(ItemAdded e) => new(
        e.ItemId,
        e.Name,
        e.Description,
        e.Tags,
        e.Quantity
    );
}

#endregion

#region ItemSummary

public record ItemSummary(Guid Id, string Name, uint Quantity);

public class ItemSummaryProjection
{
    public static ItemSummary Create(ItemAdded e) => new(
        e.ItemId,
        e.Name,
        e.Quantity
    );
}

#endregion