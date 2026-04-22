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
        }`n
        public async Task StartConsumingAsync(CancellationToken cancellationToken)
        {
            await InitializeRabbitMQAsync();
            Console.WriteLine("Waiting for messages...");
            var consumer = new AsyncEventingBasicConsumer(_channel!);
            consumer.ReceivedAsync += async (model, ea) => {
                var body = ea.Body.ToArray();
                var msg = System.Text.Json.JsonSerializer.Deserialize<BroadcastMessage>(System.Text.Encoding.UTF8.GetString(body));
                if(msg != null) { Console.WriteLine($"Message: {msg.Title}"); await _channel!.BasicAckAsync(ea.DeliveryTag, false); }
            };
            await _channel!.BasicConsumeAsync(_config.QueueName, false, consumer);
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }`n
        private async Task InitializeRabbitMQAsync()
        {
            var factory = new ConnectionFactory { HostName = _config.HostName, Port = _config.Port, UserName = _config.UserName, Password = _config.Password, VirtualHost = _config.VirtualHost };
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync(_config.ExchangeName, ExchangeType.Direct, true, false);
            await _channel.QueueDeclareAsync(_config.QueueName, true, false, false, null);
            await _channel.QueueBindAsync(_config.QueueName, _config.ExchangeName, _config.RoutingKey);
            await _channel.BasicQosAsync(0, 1, false);
        }`n
        public void Dispose() { try { _channel?.CloseAsync().Wait(); _channel?.Dispose(); _connection?.CloseAsync().Wait(); _connection?.Dispose(); } catch { } }`n    }
}