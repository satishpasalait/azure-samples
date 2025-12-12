using System.Text.Json;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Samples.EventGrid.Subscriber.Models;

namespace Azure.Samples.EventGrid.Subscriber;

public class EventGridUserHandler
{
    private readonly ILogger _logger;

    public EventGridUserHandler(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<EventGridUserHandler>();
    }

    /// <summary>
    /// Handles user-related Event Grid events
    /// </summary>
    [Function("ProcessUserEvent")]
    public async Task ProcessUserEvent(
        [EventGridTrigger] EventGridEvent eventGridEvent,
        FunctionContext context)
    {
        _logger.LogInformation($"Received User Event: {eventGridEvent.EventType}");

        try
        {
            var userEvent = JsonSerializer.Deserialize<UserEvent>(eventGridEvent.Data.ToString());

            if (userEvent != null)
            {
                _logger.LogInformation($"User ID: {userEvent.UserId}");
                _logger.LogInformation($"Username: {userEvent.UserName}");
                _logger.LogInformation($"Email: {userEvent.Email}");
                _logger.LogInformation($"Action: {userEvent.Action}");

                switch (userEvent.Action.ToLower())
                {
                    case "created":
                        await HandleUserCreated(userEvent);
                        break;
                    case "updated":
                        await HandleUserUpdated(userEvent);
                        break;
                    case "deleted":
                        await HandleUserDeleted(userEvent);
                        break;
                    default:
                        _logger.LogWarning($"Unknown user action: {userEvent.Action}");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing user event: {ex.Message}");
            throw;
        }
    }

    private async Task HandleUserCreated(UserEvent userEvent)
    {
        _logger.LogInformation($"New user created: {userEvent.UserName} ({userEvent.Email})");
        
        // Simulate processing
        await Task.Delay(100);
        
        // Here you would typically:
        // - Send welcome email
        // - Create user profile
        // - Initialize user settings
        // - etc.
    }

    private async Task HandleUserUpdated(UserEvent userEvent)
    {
        _logger.LogInformation($"User updated: {userEvent.UserName}");
        
        // Simulate processing
        await Task.Delay(100);
        
        // Here you would typically:
        // - Update user profile
        // - Sync with external systems
        // - etc.
    }

    private async Task HandleUserDeleted(UserEvent userEvent)
    {
        _logger.LogInformation($"User deleted: {userEvent.UserName}");
        
        // Simulate processing
        await Task.Delay(100);
        
        // Here you would typically:
        // - Archive user data
        // - Remove from external systems
        // - Send deletion notification
        // - etc.
    }
}

