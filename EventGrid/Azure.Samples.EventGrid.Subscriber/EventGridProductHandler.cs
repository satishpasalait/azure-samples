using System.Text.Json;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Samples.EventGrid.Subscriber.Models;

namespace Azure.Samples.EventGrid.Subscriber;

public class EventGridProductHandler
{
    private readonly ILogger _logger;

    public EventGridProductHandler(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<EventGridProductHandler>();
    }

    /// <summary>
    /// Handles product-related Event Grid events
    /// </summary>
    [Function("ProcessProductEvent")]
    public async Task ProcessProductEvent(
        [EventGridTrigger] EventGridEvent eventGridEvent,
        FunctionContext context)
    {
        _logger.LogInformation($"Received Product Event: {eventGridEvent.EventType}");

        try
        {
            var productEvent = JsonSerializer.Deserialize<ProductEvent>(eventGridEvent.Data.ToString());

            if (productEvent != null)
            {
                _logger.LogInformation($"Product ID: {productEvent.ProductId}");
                _logger.LogInformation($"Product Name: {productEvent.ProductName}");
                _logger.LogInformation($"Category: {productEvent.Category}");
                _logger.LogInformation($"Event Type: {productEvent.EventType}");

                // Log product category and stock information from event data
                _logger.LogInformation($"Product Category: {productEvent.Category}");
                _logger.LogInformation($"Stock Level: {(productEvent.StockQuantity < 10 ? "Low" : "Normal")}");

                switch (productEvent.EventType)
                {
                    case "ProductCreated":
                        await HandleProductCreated(productEvent);
                        break;
                    case "ProductUpdated":
                        await HandleProductUpdated(productEvent);
                        break;
                    case "ProductDeleted":
                        await HandleProductDeleted(productEvent);
                        break;
                    case "StockLow":
                        await HandleStockLow(productEvent);
                        break;
                    default:
                        _logger.LogWarning($"Unknown product event type: {productEvent.EventType}");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing product event: {ex.Message}");
            throw;
        }
    }

    private async Task HandleProductCreated(ProductEvent productEvent)
    {
        _logger.LogInformation($"New product created: {productEvent.ProductName} (${productEvent.Price})");
        
        // Simulate processing
        await Task.Delay(100);
        
        // Here you would typically:
        // - Index product in search
        // - Update catalog
        // - Send notifications
        // - etc.
    }

    private async Task HandleProductUpdated(ProductEvent productEvent)
    {
        _logger.LogInformation($"Product updated: {productEvent.ProductName}");
        
        // Simulate processing
        await Task.Delay(100);
        
        // Here you would typically:
        // - Update search index
        // - Invalidate cache
        // - Sync with external systems
        // - etc.
    }

    private async Task HandleProductDeleted(ProductEvent productEvent)
    {
        _logger.LogInformation($"Product deleted: {productEvent.ProductName}");
        
        // Simulate processing
        await Task.Delay(100);
        
        // Here you would typically:
        // - Remove from search index
        // - Archive product data
        // - Update catalog
        // - etc.
    }

    private async Task HandleStockLow(ProductEvent productEvent)
    {
        _logger.LogWarning($"Low stock alert for {productEvent.ProductName}: {productEvent.StockQuantity} remaining");
        
        // Simulate processing
        await Task.Delay(100);
        
        // Here you would typically:
        // - Send alert to inventory manager
        // - Trigger reorder process
        // - Update product status
        // - etc.
    }
}

