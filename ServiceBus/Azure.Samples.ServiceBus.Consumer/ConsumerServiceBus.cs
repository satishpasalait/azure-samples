using Azure.Messaging.ServiceBus;
using System.Text.Json;


public class ConsumerServiceBus
{
    private readonly string _connectionString;
    private readonly string _queueName;
    public ConsumerServiceBus(string connectionString, string queueName)
    {
        _connectionString = connectionString;
        _queueName = queueName;
    }

    public async Task ReceiveMessagesAsync()
    {
        var client = new ServiceBusClient(_connectionString);
        var processorOptions = new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = 2,
            AutoCompleteMessages = false
        };

        var processor = client.CreateProcessor(_queueName, processorOptions);

        processor.ProcessMessageAsync += MessageHandler;
        processor.ProcessErrorAsync += ErrorHandler;

        Console.WriteLine($"Receiving messages from {_queueName}...");
        await processor.StartProcessingAsync();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();

        Console.WriteLine("Stopping processor...");
        await processor.StopProcessingAsync();
        await processor.DisposeAsync();
        await client.DisposeAsync();
        Console.WriteLine("Processor stopped and resources disposed");

    }

    protected async Task MessageHandler(ProcessMessageEventArgs args)
    {
        try
        {
            var body = args.Message.Body.ToString();
            var order = JsonSerializer.Deserialize<Order>(body);

            Console.WriteLine($"Received message: {order.Id} - {order.CustomerName} - {order.Price} - {order.Quantity} - {order.OrderDate}");

            await Task.Delay(1000);

            await args.CompleteMessageAsync(args.Message);
            Console.WriteLine($"Message {args.Message.MessageId} completed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing message: {ex.Message}");

            await args.DeadLetterMessageAsync(args.Message);
            Console.WriteLine($"Message {args.Message.MessageId} dead lettered");
        }
    }

    protected async Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine($"Error: {args.Exception.Message}");
    }
}