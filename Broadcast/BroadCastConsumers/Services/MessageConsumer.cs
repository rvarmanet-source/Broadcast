using BroadCastConsumers.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace BroadCastConsumers.Services
{
    public class MessageConsumer : IDisposable
    {
        private readonly Configuration.RabbitMQConfiguration _config;
        private IConnection? _connection;
        private IChannel? _channel;
        private bool _disposed = false;

        public MessageConsumer(Configuration.RabbitMQConfiguration config)
        {
            _config = config;
        }

        public async Task StartConsumingAsync(CancellationToken cancellationToken)
        {
            await InitializeRabbitMQAsync();

            Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║     BROADCAST CONSUMERS - MESSAGE LISTENER ACTIVE             ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Connected to: {_config.HostName}:{_config.Port}");
            Console.WriteLine($"Queue: {_config.QueueName}");
            Console.WriteLine($"Exchange: {_config.ExchangeName}");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Listening for broadcast messages... (Press Ctrl+C to exit)");
            Console.WriteLine(new string('─', 65));
            Console.WriteLine();

            var consumer = new AsyncEventingBasicConsumer(_channel!);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<BroadcastMessage>(messageJson);

                    if (message != null)
                    {
                        await ProcessMessageAsync(message, ea);
                        await _channel!.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("❌ Received null message - rejecting");
                        Console.ResetColor();
                        await _channel!.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Error processing message: {ex.Message}");
                    Console.ResetColor();
                    await _channel!.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await _channel!.BasicConsumeAsync(
                queue: _config.QueueName,
                autoAck: false,
                consumer: consumer);

            // Keep the consumer running until cancellation
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }

        private async Task InitializeRabbitMQAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _config.HostName,
                Port = _config.Port,
                UserName = _config.UserName,
                Password = _config.Password,
                VirtualHost = _config.VirtualHost
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            // Declare exchange
            await _channel.ExchangeDeclareAsync(
                exchange: _config.ExchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);

            // Declare queue
            await _channel.QueueDeclareAsync(
                queue: _config.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Bind queue to exchange
            await _channel.QueueBindAsync(
                queue: _config.QueueName,
                exchange: _config.ExchangeName,
                routingKey: _config.RoutingKey);

            // Set QoS - process one message at a time
            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);
        }

        private async Task ProcessMessageAsync(BroadcastMessage message, BasicDeliverEventArgs ea)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            Console.ForegroundColor = GetPriorityColor(message.Priority);
            Console.WriteLine($"┌─ Message Received at {timestamp} ─────────────────────────");
            Console.ResetColor();

            Console.WriteLine($"│ ID:         {message.Id}");
            Console.WriteLine($"│ Title:      {message.Title}");
            Console.WriteLine($"│ Content:    {message.Content}");
            Console.WriteLine($"│ Priority:   {message.Priority}");
            Console.WriteLine($"│ Status:     {message.Status}");
            Console.WriteLine($"│ Category:   {message.Category ?? "N/A"}");
            Console.WriteLine($"│ Created By: {message.CreatedBy}");
            Console.WriteLine($"│ Created At: {message.CreatedAt:yyyy-MM-dd HH:mm:ss}");

            if (message.ScheduledFor.HasValue)
                Console.WriteLine($"│ Scheduled:  {message.ScheduledFor:yyyy-MM-dd HH:mm:ss}");

            Console.ForegroundColor = GetPriorityColor(message.Priority);
            Console.WriteLine($"└────────────────────────────────────────────────────────────");
            Console.ResetColor();
            Console.WriteLine();

            // Add your custom message processing logic here
            await Task.CompletedTask;
        }

        private ConsoleColor GetPriorityColor(MessagePriority priority)
        {
            return priority switch
            {
                MessagePriority.Urgent => ConsoleColor.Red,
                MessagePriority.High => ConsoleColor.Yellow,
                MessagePriority.Normal => ConsoleColor.Green,
                MessagePriority.Low => ConsoleColor.Gray,
                _ => ConsoleColor.White
            };
        }

        public void Dispose()
        {
            if (_disposed) return;

            _channel?.Dispose();
            _connection?.Dispose();
            _disposed = true;

            Console.WriteLine("\nConnection closed. Consumer stopped.");
        }
    }
}