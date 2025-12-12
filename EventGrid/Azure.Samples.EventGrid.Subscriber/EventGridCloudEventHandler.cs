using System.Text.Json;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Azure.Samples.EventGrid.Subscriber;

public class EventGridCloudEventHandler
{
    private readonly ILogger _logger;

    public EventGridCloudEventHandler(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<EventGridCloudEventHandler>();
    }

    /// <summary>
    /// Handles CloudEvents schema events
    /// </summary>
    [Function("ProcessCloudEvent")]
    public async Task ProcessCloudEvent(
        [EventGridTrigger] CloudEvent cloudEvent,
        FunctionContext context)
    {
        _logger.LogInformation($"Received CloudEvent: {cloudEvent.Type}");
        _logger.LogInformation($"CloudEvent Source: {cloudEvent.Source}");
        _logger.LogInformation($"CloudEvent Subject: {cloudEvent.Subject}");
        _logger.LogInformation($"CloudEvent ID: {cloudEvent.Id}");
        _logger.LogInformation($"CloudEvent Time: {cloudEvent.Time}");

        try
        {
            // Process CloudEvent data
            if (cloudEvent.Data != null)
            {
                var dataString = cloudEvent.Data.ToString();
                _logger.LogInformation($"CloudEvent Data: {dataString}");

                // Deserialize based on event type
                var eventData = JsonSerializer.Deserialize<JsonElement>(dataString);
                
                // Process the event
                await ProcessCloudEventData(cloudEvent, eventData);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing CloudEvent: {ex.Message}");
            throw;
        }
    }

    private async Task ProcessCloudEventData(CloudEvent cloudEvent, JsonElement eventData)
    {
        _logger.LogInformation($"Processing CloudEvent of type: {cloudEvent.Type}");

        // Handle different CloudEvent types
        if (cloudEvent.Type.Contains("Message"))
        {
            if (eventData.TryGetProperty("Message", out var message))
            {
                _logger.LogInformation($"Message: {message.GetString()}");
            }
            if (eventData.TryGetProperty("Priority", out var priority))
            {
                _logger.LogInformation($"Priority: {priority.GetString()}");
            }
        }

        await Task.CompletedTask;
    }
}

