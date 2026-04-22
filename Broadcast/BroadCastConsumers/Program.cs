using BroadCastConsumers.Configuration;
using BroadCastConsumers.Services;

Console.WriteLine("Starting BroadCast Consumers Application...");
Console.WriteLine();

// Configure RabbitMQ settings
var config = new RabbitMQConfiguration
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest",
    VirtualHost = "/",
    QueueName = "broadcast-messages",
    ExchangeName = "(AMQP default)",
    RoutingKey = "broadcast"
};

// Create cancellation token for graceful shutdown
using var cts = new CancellationTokenSource();

Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    cts.Cancel();
    Console.WriteLine("\nShutting down consumer...");
};

try
{
    using var consumer = new MessageConsumer(config);
    await consumer.StartConsumingAsync(cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Consumer stopped gracefully.");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Fatal error: {ex.Message}");
    Console.ResetColor();
    return 1;
}

return 0;
