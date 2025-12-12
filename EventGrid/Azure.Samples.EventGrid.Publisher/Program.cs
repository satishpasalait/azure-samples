using Azure.Samples.EventGrid.Publisher;
using Azure.Samples.EventGrid.Publisher.Models;

var topicEndpoint = Environment.GetEnvironmentVariable("EVENT_GRID_TOPIC_ENDPOINT") 
    ?? throw new InvalidOperationException("EVENT_GRID_TOPIC_ENDPOINT environment variable is required");

var topicKey = Environment.GetEnvironmentVariable("EVENT_GRID_TOPIC_KEY") 
    ?? throw new InvalidOperationException("EVENT_GRID_TOPIC_KEY environment variable is required");

var publisher = new EventGridPublisher(topicEndpoint, topicKey);

Console.WriteLine("Azure Event Grid Publisher Sample");
Console.WriteLine("==================================");
Console.WriteLine();

try
{
    // Example 1: Publish a single custom order event
    Console.WriteLine("1. Publishing Order Created event...");
    var orderCreated = new OrderEvent
    {
        OrderId = 1001,
        CustomerName = "John Doe",
        TotalAmount = 1250.50m,
        ItemCount = 3,
        OrderDate = DateTime.UtcNow,
        Status = "Created",
        Notes = "Priority order"
    };
    await publisher.PublishOrderEventAsync(orderCreated);
    Console.WriteLine();

    // Example 2: Publish order status change events
    Console.WriteLine("2. Publishing Order Status Change events...");
    var orderProcessing = new OrderEvent
    {
        OrderId = 1001,
        CustomerName = "John Doe",
        TotalAmount = 1250.50m,
        ItemCount = 3,
        OrderDate = DateTime.UtcNow,
        Status = "Processing"
    };
    await publisher.PublishOrderEventAsync(orderProcessing);
    await Task.Delay(500);

    var orderShipped = new OrderEvent
    {
        OrderId = 1001,
        CustomerName = "John Doe",
        TotalAmount = 1250.50m,
        ItemCount = 3,
        OrderDate = DateTime.UtcNow,
        Status = "Shipped"
    };
    await publisher.PublishOrderEventAsync(orderShipped);
    Console.WriteLine();

    // Example 3: Publish user events
    Console.WriteLine("3. Publishing User events...");
    var userCreated = new UserEvent
    {
        UserId = "user-123",
        UserName = "jane.smith",
        Email = "jane.smith@example.com",
        Action = "Created",
        Timestamp = DateTime.UtcNow
    };
    await publisher.PublishUserEventAsync(userCreated);
    await Task.Delay(500);

    var userUpdated = new UserEvent
    {
        UserId = "user-123",
        UserName = "jane.smith",
        Email = "jane.smith.updated@example.com",
        Action = "Updated",
        Timestamp = DateTime.UtcNow
    };
    await publisher.PublishUserEventAsync(userUpdated);
    Console.WriteLine();

    // Example 4: Publish product events
    Console.WriteLine("4. Publishing Product events...");
    var productCreated = new ProductEvent
    {
        ProductId = "prod-456",
        ProductName = "Laptop Pro 15",
        Price = 1299.99m,
        StockQuantity = 50,
        Category = "Electronics",
        EventType = "ProductCreated",
        Timestamp = DateTime.UtcNow
    };
    await publisher.PublishProductEventAsync(productCreated);
    await Task.Delay(500);

    var stockLow = new ProductEvent
    {
        ProductId = "prod-789",
        ProductName = "Wireless Mouse",
        Price = 29.99m,
        StockQuantity = 5,
        Category = "Accessories",
        EventType = "StockLow",
        Timestamp = DateTime.UtcNow
    };
    await publisher.PublishProductEventAsync(stockLow);
    Console.WriteLine();

    // Example 5: Publish batch of events
    Console.WriteLine("5. Publishing batch of events...");
    var orders = new List<OrderEvent>
    {
        new OrderEvent { OrderId = 2001, CustomerName = "Alice Brown", TotalAmount = 450.00m, ItemCount = 2, OrderDate = DateTime.UtcNow, Status = "Created" },
        new OrderEvent { OrderId = 2002, CustomerName = "Bob Wilson", TotalAmount = 890.75m, ItemCount = 5, OrderDate = DateTime.UtcNow, Status = "Created" },
        new OrderEvent { OrderId = 2003, CustomerName = "Carol Davis", TotalAmount = 320.50m, ItemCount = 1, OrderDate = DateTime.UtcNow, Status = "Created" }
    };
    await publisher.PublishEventsBatchAsync(orders, "Order", "orders");
    Console.WriteLine();

    // Example 6: Publish CloudEvent
    Console.WriteLine("6. Publishing CloudEvent...");
    var cloudEventData = new { Message = "This is a CloudEvent", Priority = "high" };
    await publisher.PublishCloudEventAsync(
        cloudEventData,
        "com.example.CloudEvent",
        "cloud-events/sample",
        "https://example.com/source"
    );
    Console.WriteLine();

    // Example 7: Publish event with advanced filtering attributes
    Console.WriteLine("7. Publishing event with filtering attributes...");
    var filteredEvent = new { Data = "Filtered event data", Region = "US-East", Environment = "Production" };
    var filterAttributes = new Dictionary<string, string>
    {
        { "region", "US-East" },
        { "environment", "Production" },
        { "priority", "high" }
    };
    await publisher.PublishEventWithFilteringAsync(filteredEvent, "Custom.FilteredEvent", "filtered/sample", filterAttributes);
    Console.WriteLine();

    Console.WriteLine("All events published successfully!");
}
catch (Exception ex)
{
    Console.WriteLine($"Error publishing events: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}
finally
{
    publisher.Dispose();
}

Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadKey();

