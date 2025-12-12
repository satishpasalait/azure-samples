using System.Text.Json;
using Azure.Messaging.ServiceBus;

public class ProducerServiceBus
{
    private readonly string _connectionString;
    private readonly string _queueName;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;
    public ProducerServiceBus(string connectionString, string queueName)
    {
        _connectionString = connectionString;
        _queueName = queueName;
        _client = new ServiceBusClient(_connectionString);
        _sender = _client.CreateSender(_queueName);
    }

    public async Task SendMessageAsync(Order order)
    {
        var message = new ServiceBusMessage(JsonSerializer.Serialize(order));
        await _sender.SendMessageAsync(message);
    }

    public async Task CloseAsync()
    {
        await _sender.CloseAsync();
        await _client.DisposeAsync();
    }
}