using Broadcast.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Broadcast.Services
{
    public class BroadcastMessageConsumer : BackgroundService
    {
        private readonly RabbitMQConfiguration _config;
        private readonly ILogger<BroadcastMessageConsumer> _logger;
        private IConnection? _connection;
        private IChannel? _channel;

        public BroadcastMessageConsumer(
            IOptions<RabbitMQConfiguration> config,
            ILogger<BroadcastMessageConsumer> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await InitializeRabbitMQ(stoppingToken);

            stoppingToken.Register(() =>
            {
                _logger.LogInformation("RabbitMQ Consumer is stopping");
                CleanupAsync().GetAwaiter().GetResult();
            });

            _logger.LogInformation("RabbitMQ Consumer started and listening for messages");
        }

        private async Task InitializeRabbitMQ(CancellationToken stoppingToken)
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

                // Set QoS to process one message at a time
                await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

                // Create consumer
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var messageJson = Encoding.UTF8.GetString(body);
                        var message = JsonSerializer.Deserialize<BroadcastMessage>(messageJson);

                        if (message != null)
                        {
                            await ProcessMessageAsync(message);
                            await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                            _logger.LogInformation("Message processed and acknowledged - ID: {MessageId}", message.Id);
                        }
                        else
                        {
                            _logger.LogWarning("Received null message, rejecting");
                            await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message");
                        await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                    }
                };

                await _channel.BasicConsumeAsync(
                    queue: _config.QueueName,
                    autoAck: false,
                    consumer: consumer);

                _logger.LogInformation("RabbitMQ Consumer initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize RabbitMQ Consumer");
                throw;
            }
        }

        private async Task ProcessMessageAsync(BroadcastMessage message)
        {
            // Implement your message processing logic here
            // Examples:
            // - Send email
            // - Send SMS
            // - Send push notification
            // - Log to database
            // - Trigger other services

            _logger.LogInformation(
                "Processing broadcast message - ID: {MessageId}, Title: {Title}, Priority: {Priority}, Status: {Status}",
                message.Id, message.Title, message.Priority, message.Status);

            // Simulate processing
            await Task.Delay(100);

            // Example: You could call different services based on message type
            switch (message.Priority)
            {
                case MessagePriority.Urgent:
                    _logger.LogWarning("URGENT MESSAGE: {Title} - {Content}", message.Title, message.Content);
                    // Send immediate notification
                    break;
                case MessagePriority.High:
                    _logger.LogInformation("HIGH PRIORITY: {Title}", message.Title);
                    // Send high priority notification
                    break;
                default:
                    _logger.LogInformation("Standard message: {Title}", message.Title);
                    // Send normal notification
                    break;
            }
        }

        private async Task CleanupAsync()
        {
            try
            {
                if (_channel != null)
                {
                    await _channel.CloseAsync();
                    _channel.Dispose();
                }

                if (_connection != null)
                {
                    await _connection.CloseAsync();
                    _connection.Dispose();
                }

                _logger.LogInformation("RabbitMQ Consumer cleaned up");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cleanup");
            }
        }

        public override void Dispose()
        {
            CleanupAsync().GetAwaiter().GetResult();
            base.Dispose();
        }
    }
}
