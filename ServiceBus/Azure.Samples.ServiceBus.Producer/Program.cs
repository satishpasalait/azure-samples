

var connectionString = Environment.GetEnvironmentVariable("SERVICE_BUS_CONNECTION_STRING");
var queueName = Environment.GetEnvironmentVariable("SERVICE_BUS_QUEUE_NAME");

var producer = new ProducerServiceBus(connectionString, queueName);

Console.WriteLine($"Sending messages to {queueName}...");

for (int i = 0; i < 10; i++)
{
    var order = new Order
    {
        Id = i,
        CustomerName = $"Customer {i}",
        Price = 100 + (i * 10),
        Quantity = 1 + (i * 2),
        OrderDate = DateTime.UtcNow
    };
    await producer.SendMessageAsync(order);
}

await producer.CloseAsync();

Console.WriteLine("Messages sent successfully");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();