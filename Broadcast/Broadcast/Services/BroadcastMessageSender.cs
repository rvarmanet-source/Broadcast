using Broadcast.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Broadcast.Services
{
    public class BroadcastMessageSender : IBroadcastMessageSender, IDisposable
    {
        private readonly RabbitMQConfiguration _config;
        private readonly ILogger<BroadcastMessageSender> _logger;
        private IConnection? _connection;
        private IChannel? _channel;
        private bool _disposed = false;

        public BroadcastMessageSender(
            IOptions<RabbitMQConfiguration> config,
            ILogger<BroadcastMessageSender> logger)
        {
            _config = config.Value;
            _logger = logger;
            InitializeRabbitMQ().GetAwaiter().GetResult();
        }

        private async Task InitializeRabbitMQ()
        {
            try
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
                    durable: _config.DurableQueue,
                    exclusive: false,
                    autoDelete: _config.AutoDeleteQueue,
                    arguments: null);

                // Bind queue to exchange
                await _channel.QueueBindAsync(
                    queue: _config.QueueName,
                    exchange: _config.ExchangeName,
                    routingKey: _config.RoutingKey);

                _logger.LogInformation("RabbitMQ connection initialized successfully. Queue: {QueueName}, Exchange: {ExchangeName}",
                    _config.QueueName, _config.ExchangeName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize RabbitMQ connection");
                throw;
            }
        }

        public async Task SendMessageAsync(BroadcastMessage message)
        {
            if (_channel == null)
            {
                _logger.LogError("RabbitMQ channel is not initialized");
                throw new InvalidOperationException("RabbitMQ channel is not initialized");
            }

            try
            {
                var messageJson = JsonSerializer.Serialize(message, new JsonSerializerOptions
                {
                    WriteIndented = false
                });

                var body = Encoding.UTF8.GetBytes(messageJson);

                var properties = new BasicProperties
                {
                    Persistent = true,
                    ContentType = "application/json",
                    MessageId = message.Id.ToString(),
                    Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
                    Headers = new Dictionary<string, object?>
                    {
                        { "priority", (int)message.Priority },
                        { "status", message.Status.ToString() },
                        { "category", message.Category ?? string.Empty }
                    }
                };

                await _channel.BasicPublishAsync(
                    exchange: _config.ExchangeName,
                    routingKey: _config.RoutingKey,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation("Message published to RabbitMQ - ID: {MessageId}, Title: {Title}, Priority: {Priority}",
                    message.Id, message.Title, message.Priority);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message to RabbitMQ - ID: {MessageId}", message.Id);
                throw;
            }
        }

        public async Task SendMessagesAsync(IEnumerable<BroadcastMessage> messages)
        {
            foreach (var message in messages)
            {
                await SendMessageAsync(message);
            }
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
                        _logger.LogInformation("RabbitMQ connection disposed");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error disposing RabbitMQ connection");
                    }
                }
                _disposed = true;
            }
        }
    }
}
