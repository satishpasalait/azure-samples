using System.Text.Json;
using Azure.Core;
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
    /// Handles CloudEvents-style events (using EventGridEvent with CloudEvents-compatible data structure)
    /// Note: CloudEvent type may not be available in all versions. Using EventGridEvent instead.
    /// </summary>
    [Function("ProcessCloudEvent")]
    public async Task ProcessCloudEvent(
        [EventGridTrigger] EventGridEvent eventGridEvent,
        FunctionContext context)
    {
        _logger.LogInformation($"Received CloudEvent-style event: {eventGridEvent.EventType}");
        _logger.LogInformation($"Event Subject: {eventGridEvent.Subject}");
        _logger.LogInformation($"Event ID: {eventGridEvent.Id}");
        _logger.LogInformation($"Event Time: {eventGridEvent.EventTime}");

        try
        {
            // Process event data
            if (eventGridEvent.Data != null)
            {
                var dataString = eventGridEvent.Data.ToString();
                _logger.LogInformation($"Event Data: {dataString}");

                // Deserialize the data
                var eventData = JsonSerializer.Deserialize<JsonElement>(dataString);
                
                // Process the event
                await ProcessCloudEventData(eventGridEvent, eventData);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing CloudEvent-style event: {ex.Message}");
            throw;
        }
    }

    private async Task ProcessCloudEventData(EventGridEvent eventGridEvent, JsonElement eventData)
    {
        _logger.LogInformation($"Processing CloudEvent-style event of type: {eventGridEvent.EventType}");

        // Handle different event types
        if (eventGridEvent.EventType.Contains("Message") || eventGridEvent.EventType.Contains("CloudEvent"))
        {
            // Try to extract nested data if it's in CloudEvents format
            if (eventData.TryGetProperty("data", out var nestedData))
            {
                if (nestedData.TryGetProperty("Message", out var message))
                {
                    _logger.LogInformation($"Message: {message.GetString()}");
                }
                if (nestedData.TryGetProperty("Priority", out var priority))
                {
                    _logger.LogInformation($"Priority: {priority.GetString()}");
                }
            }
            else
            {
                // Direct properties
                if (eventData.TryGetProperty("Message", out var message))
                {
                    _logger.LogInformation($"Message: {message.GetString()}");
                }
                if (eventData.TryGetProperty("Priority", out var priority))
                {
                    _logger.LogInformation($"Priority: {priority.GetString()}");
                }
            }
        }

        await Task.CompletedTask;
    }
}

