using BroadcastConsumer.Configuration;
using BroadcastConsumer.Services;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var rabbitMQConfig = new RabbitMQConfiguration();
        configuration.GetSection("RabbitMQ").Bind(rabbitMQConfig);

        Console.Clear();
        using var cts = new CancellationTokenSource();
        using var consumer = new MessageConsumer(rabbitMQConfig);
        await consumer.StartConsumingAsync(cts.Token);
    }
}