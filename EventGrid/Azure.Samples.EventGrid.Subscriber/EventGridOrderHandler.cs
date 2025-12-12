using System.Text.Json;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Samples.EventGrid.Subscriber.Models;

namespace Azure.Samples.EventGrid.Subscriber;

public class EventGridOrderHandler
{
    private readonly ILogger _logger;

    public EventGridOrderHandler(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<EventGridOrderHandler>();
    }

    /// <summary>
    /// Handles Event Grid events for orders using EventGridEvent binding
    /// </summary>
    [Function("ProcessOrderEvent")]
    public async Task ProcessOrderEvent(
        [EventGridTrigger] EventGridEvent eventGridEvent,
        FunctionContext context)
    {
        _logger.LogInformation($"Received Event Grid event: {eventGridEvent.EventType}");
        _logger.LogInformation($"Event Subject: {eventGridEvent.Subject}");
        _logger.LogInformation($"Event ID: {eventGridEvent.Id}");
        _logger.LogInformation($"Event Time: {eventGridEvent.EventTime}");

        try
        {
            // Deserialize the event data
            var orderEvent = JsonSerializer.Deserialize<OrderEvent>(eventGridEvent.Data.ToString());
            
            if (orderEvent != null)
            {
                _logger.LogInformation($"Processing Order Event - OrderId: {orderEvent.OrderId}, Status: {orderEvent.Status}, Customer: {orderEvent.CustomerName}");

                // Process based on order status
                switch (orderEvent.Status.ToLower())
                {
                    case "created":
                        await HandleOrderCreated(orderEvent);
                        break;
                    case "processing":
                        await HandleOrderProcessing(orderEvent);
                        break;
                    case "shipped":
                        await HandleOrderShipped(orderEvent);
                        break;
                    case "delivered":
                        await HandleOrderDelivered(orderEvent);
                        break;
                    case "cancelled":
                        await HandleOrderCancelled(orderEvent);
                        break;
                    default:
                        _logger.LogWarning($"Unknown order status: {orderEvent.Status}");
                        break;
                }

                // Log event metadata
                _logger.LogInformation($"Event Metadata - ID: {eventGridEvent.Id}, Time: {eventGridEvent.EventTime}");
            }
            else
            {
                _logger.LogWarning("Failed to deserialize order event data");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing order event: {ex.Message}");
            throw; // Re-throw to trigger retry or dead letter
        }
    }

    /// <summary>
    /// Handles Event Grid events using string binding for custom processing
    /// </summary>
    [Function("ProcessEventGridEvent")]
    public async Task ProcessEventGridEvent(
        [EventGridTrigger] string eventGridEventJson,
        FunctionContext context)
    {
        _logger.LogInformation("Received Event Grid event (string binding)");

        try
        {
            var eventGridEvent = JsonSerializer.Deserialize<EventGridEvent>(eventGridEventJson);
            
            if (eventGridEvent != null)
            {
                _logger.LogInformation($"Event Type: {eventGridEvent.EventType}");
                _logger.LogInformation($"Subject: {eventGridEvent.Subject}");
                _logger.LogInformation($"Data: {eventGridEvent.Data}");

                // Process the event based on type
                await ProcessEventByType(eventGridEvent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing event: {ex.Message}");
            throw;
        }
    }

    private async Task HandleOrderCreated(OrderEvent orderEvent)
    {
        _logger.LogInformation($"Order {orderEvent.OrderId} created for customer {orderEvent.CustomerName}");
        _logger.LogInformation($"Total Amount: ${orderEvent.TotalAmount}, Items: {orderEvent.ItemCount}");
        
        // Simulate processing
        await Task.Delay(100);
        
        // Here you would typically:
        // - Send confirmation email
        // - Update inventory
        // - Create shipping label
        // - etc.
    }

    private async Task HandleOrderProcessing(OrderEvent orderEvent)
    {
        _logger.LogInformation($"Order {orderEvent.OrderId} is being processed");
        
        // Simulate processing
        await Task.Delay(100);
    }

    private async Task HandleOrderShipped(OrderEvent orderEvent)
    {
        _logger.LogInformation($"Order {orderEvent.OrderId} has been shipped");
        
        // Simulate processing
        await Task.Delay(100);
        
        // Here you would typically:
        // - Send shipping notification
        // - Update tracking information
        // - etc.
    }

    private async Task HandleOrderDelivered(OrderEvent orderEvent)
    {
        _logger.LogInformation($"Order {orderEvent.OrderId} has been delivered");
        
        // Simulate processing
        await Task.Delay(100);
    }

    private async Task HandleOrderCancelled(OrderEvent orderEvent)
    {
        _logger.LogWarning($"Order {orderEvent.OrderId} has been cancelled");
        
        // Simulate processing
        await Task.Delay(100);
        
        // Here you would typically:
        // - Process refund
        // - Restore inventory
        // - Send cancellation notification
        // - etc.
    }

    private async Task ProcessEventByType(EventGridEvent eventGridEvent)
    {
        var eventType = eventGridEvent.EventType;

        if (eventType.StartsWith("Order."))
        {
            var orderEvent = JsonSerializer.Deserialize<OrderEvent>(eventGridEvent.Data.ToString());
            if (orderEvent != null)
            {
                _logger.LogInformation($"Processing order event: {orderEvent.OrderId}");
            }
        }
        else if (eventType.StartsWith("User."))
        {
            var userEvent = JsonSerializer.Deserialize<UserEvent>(eventGridEvent.Data.ToString());
            if (userEvent != null)
            {
                _logger.LogInformation($"Processing user event: {userEvent.UserId} - {userEvent.Action}");
            }
        }
        else if (eventType.Contains("Product"))
        {
            var productEvent = JsonSerializer.Deserialize<ProductEvent>(eventGridEvent.Data.ToString());
            if (productEvent != null)
            {
                _logger.LogInformation($"Processing product event: {productEvent.ProductId} - {productEvent.EventType}");
                
                // Handle stock low alerts
                if (productEvent.EventType == "StockLow")
                {
                    _logger.LogWarning($"Low stock alert for product {productEvent.ProductName}: {productEvent.StockQuantity} remaining");
                }
            }
        }
        else
        {
            _logger.LogInformation($"Processing generic event type: {eventType}");
        }

        await Task.CompletedTask;
    }
}

