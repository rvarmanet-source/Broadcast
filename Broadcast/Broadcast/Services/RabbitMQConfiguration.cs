namespace Broadcast.Services
{
    public class RabbitMQConfiguration
    {
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public string QueueName { get; set; } = "broadcast-messages";
        public string ExchangeName { get; set; } = "(AMQP default)";
        public string RoutingKey { get; set; } = "broadcast.message";
        public bool DurableQueue { get; set; } = true;
        public bool AutoDeleteQueue { get; set; } = false;
    }
}
