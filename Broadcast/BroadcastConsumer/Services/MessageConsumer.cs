using BroadcastConsumer.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace BroadcastConsumer.Services
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
            Console.WriteLine("║        BROADCAST MESSAGE CONSUMER - ACTIVE                    ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Connected to: {_config.HostName}:{_config.Port}");
            Console.WriteLine($"Queue: {_config.QueueName}");
            Console.WriteLine($"Exchange: {_config.ExchangeName}");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Waiting for broadcast messages... (Press Ctrl+C to exit)");
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
                        DisplayMessage(message, ea);
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

            // Declare exchange (should already exist from publisher)
            await _channel.ExchangeDeclareAsync(
                exchange: _config.ExchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);

            // Declare queue (should already exist from publisher)
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

        private void DisplayMessage(BroadcastMessage message, BasicDeliverEventArgs ea)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // Message header
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("┌─────────────────────────────────────────────────────────────┐");
            Console.WriteLine($"│ NEW BROADCAST MESSAGE RECEIVED - {timestamp}    │");
            Console.WriteLine("└─────────────────────────────────────────────────────────────┘");
            Console.ResetColor();

            // Message ID and Title
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"📨 Message ID: {message.Id}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"📌 Title: {message.Title}");
            Console.ResetColor();

            // Priority with color coding
            Console.Write("⚡ Priority: ");
            switch (message.Priority)
            {
                case MessagePriority.Urgent:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("🔴 URGENT");
                    break;
                case MessagePriority.High:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("🟠 HIGH");
                    break;
                case MessagePriority.Normal:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("🔵 NORMAL");
                    break;
                case MessagePriority.Low:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("⚫ LOW");
                    break;
            }
            Console.ResetColor();

            // Status
            Console.Write("📊 Status: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message.Status);
            Console.ResetColor();

            // Category
            if (!string.IsNullOrEmpty(message.Category))
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"🏷️  Category: {message.Category}");
                Console.ResetColor();
            }

            // Content
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("─────────────────────────────────────────────────────────────");
            Console.WriteLine("📄 Content:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message.Content);
            Console.ResetColor();
            Console.WriteLine("─────────────────────────────────────────────────────────────");

            // Metadata
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"👤 Created By: {message.CreatedBy}");
            Console.WriteLine($"🕒 Created At: {message.CreatedAt:yyyy-MM-dd HH:mm:ss}");
            if (message.SentAt.HasValue)
            {
                Console.WriteLine($"📤 Sent At: {message.SentAt.Value:yyyy-MM-dd HH:mm:ss}");
            }
            if (message.ScheduledFor.HasValue)
            {
                Console.WriteLine($"⏰ Scheduled For: {message.ScheduledFor.Value:yyyy-MM-dd HH:mm:ss}");
            }
            Console.WriteLine($"👁️  View Count: {message.ViewCount}");
            Console.ResetColor();

            // RabbitMQ headers
            if (ea.BasicProperties.Headers != null && ea.BasicProperties.Headers.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine();
                Console.WriteLine("📋 RabbitMQ Headers:");
                foreach (var header in ea.BasicProperties.Headers)
                {
                    var value = Encoding.UTF8.GetString((byte[])header.Value);
                    Console.WriteLine($"   {header.Key}: {value}");
                }
                Console.ResetColor();
            }

            // Footer
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("✅ Message processed successfully");
            Console.ResetColor();
            Console.WriteLine(new string('═', 65));
            Console.WriteLine();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        _channel?.CloseAsync().GetAwaiter().GetResult();
                        _channel?.Dispose();
                        _connection?.CloseAsync().GetAwaiter().GetResult();
                        _connection?.Dispose();
                        Console.WriteLine("RabbitMQ connection closed gracefully");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error disposing RabbitMQ connection: {ex.Message}");
                    }
                }
                _disposed = true;
            }
        }
    }
}
