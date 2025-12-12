using Azure;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.Models;
using System.Text.Json;
using Azure.Samples.EventGrid.Publisher.Models;

namespace Azure.Samples.EventGrid.Publisher;

public class EventGridPublisher
{
    private readonly EventGridPublisherClient _client;
    private readonly string _topicEndpoint;
    private readonly string _topicKey;

    public EventGridPublisher(string topicEndpoint, string topicKey)
    {
        _topicEndpoint = topicEndpoint;
        _topicKey = topicKey;
        
        // Create the client using the endpoint and access key
        var credential = new AzureKeyCredential(topicKey);
        _client = new EventGridPublisherClient(new Uri(topicEndpoint), credential);
    }

    /// <summary>
    /// Publishes a custom event using Event Grid schema
    /// </summary>
    public async Task PublishCustomEventAsync<T>(T eventData, string eventType, string subject)
    {
        var customEvent = new EventGridEvent(
            subject: subject,
            eventType: eventType,
            dataVersion: "1.0",
            data: new BinaryData(JsonSerializer.Serialize(eventData))
        );

        await _client.SendEventAsync(customEvent);
        Console.WriteLine($"Published custom event: {eventType} with subject: {subject}");
    }

    /// <summary>
    /// Publishes multiple events in a single batch
    /// </summary>
    public async Task PublishEventsBatchAsync<T>(IEnumerable<T> events, string eventTypePrefix, string subjectPrefix)
    {
        var eventGridEvents = events.Select((evt, index) => new EventGridEvent(
            subject: $"{subjectPrefix}/{index}",
            eventType: $"{eventTypePrefix}.{typeof(T).Name}",
            dataVersion: "1.0",
            data: new BinaryData(JsonSerializer.Serialize(evt))
        )).ToList();

        await _client.SendEventsAsync(eventGridEvents);
        Console.WriteLine($"Published batch of {eventGridEvents.Count} events");
    }

    /// <summary>
    /// Publishes an event using CloudEvents schema
    /// </summary>
    public async Task PublishCloudEventAsync<T>(T eventData, string eventType, string subject, string source)
    {
        var cloudEvent = new CloudEvent(
            source: new Uri(source),
            type: eventType,
            jsonSerializableData: new BinaryData(JsonSerializer.Serialize(eventData))
        )
        {
            Subject = subject,
            Id = Guid.NewGuid().ToString(),
            Time = DateTimeOffset.UtcNow
        };

        await _client.SendEventAsync(cloudEvent);
        Console.WriteLine($"Published CloudEvent: {eventType} with subject: {subject}");
    }

    /// <summary>
    /// Publishes an order event with custom metadata
    /// Note: Custom attributes for filtering should be included in the event data payload
    /// or used in the subject. Advanced filtering can be configured in Event Grid subscriptions.
    /// </summary>
    public async Task PublishOrderEventAsync(OrderEvent orderEvent)
    {
        var customEvent = new EventGridEvent(
            subject: $"orders/{orderEvent.OrderId}",
            eventType: $"Order.{orderEvent.Status}",
            dataVersion: "1.0",
            data: new BinaryData(JsonSerializer.Serialize(orderEvent))
        );

        await _client.SendEventAsync(customEvent);
        Console.WriteLine($"Published order event: Order {orderEvent.OrderId} - Status: {orderEvent.Status}");
    }

    /// <summary>
    /// Publishes a user event
    /// </summary>
    public async Task PublishUserEventAsync(UserEvent userEvent)
    {
        var customEvent = new EventGridEvent(
            subject: $"users/{userEvent.UserId}",
            eventType: $"User.{userEvent.Action}",
            dataVersion: "1.0",
            data: new BinaryData(JsonSerializer.Serialize(userEvent))
        );

        await _client.SendEventAsync(customEvent);
        Console.WriteLine($"Published user event: User {userEvent.UserId} - Action: {userEvent.Action}");
    }

    /// <summary>
    /// Publishes a product event
    /// Note: Filtering by category and stock level can be done using advanced filters
    /// on the event data properties in Event Grid subscriptions.
    /// </summary>
    public async Task PublishProductEventAsync(ProductEvent productEvent)
    {
        var customEvent = new EventGridEvent(
            subject: $"products/{productEvent.ProductId}",
            eventType: productEvent.EventType,
            dataVersion: "1.0",
            data: new BinaryData(JsonSerializer.Serialize(productEvent))
        );

        await _client.SendEventAsync(customEvent);
        Console.WriteLine($"Published product event: {productEvent.EventType} - Product: {productEvent.ProductName}");
    }

    /// <summary>
    /// Publishes events with data structured for advanced filtering
    /// Note: Advanced filtering is configured in Event Grid subscriptions using
    /// filters on event data properties. Structure your event data to include
    /// properties that can be filtered (e.g., region, environment, priority).
    /// </summary>
    public async Task PublishEventWithFilteringAsync<T>(T eventData, string eventType, string subject, Dictionary<string, string> filterAttributes)
    {
        var customEvent = new EventGridEvent(
            subject: subject,
            eventType: eventType,
            dataVersion: "1.0",
            data: new BinaryData(JsonSerializer.Serialize(eventData))
        );

        await _client.SendEventAsync(customEvent);
        Console.WriteLine($"Published event: {eventType} (filtering configured in subscription)");
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}

