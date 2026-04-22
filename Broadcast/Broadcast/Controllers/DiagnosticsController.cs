using Broadcast.Models;
using Broadcast.Services;
using Microsoft.AspNetCore.Mvc;

namespace Broadcast.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiagnosticsController : ControllerBase
    {
        private readonly IBroadcastMessageSender _messageSender;
        private readonly ILogger<DiagnosticsController> _logger;

        public DiagnosticsController(
            IBroadcastMessageSender messageSender,
            ILogger<DiagnosticsController> logger)
        {
            _messageSender = messageSender;
            _logger = logger;
        }

        /// <summary>
        /// Test RabbitMQ connectivity and message sending
        /// GET: api/diagnostics/test-rabbitmq
        /// </summary>
        [HttpGet("test-rabbitmq")]
        public async Task<IActionResult> TestRabbitMQ()
        {
            try
            {
                var testMessage = new BroadcastMessage
                {
                    Id = 999,
                    Title = "RabbitMQ Test Message",
                    Content = "This is a test message to verify RabbitMQ integration",
                    Priority = MessagePriority.Normal,
                    Status = MessageStatus.Sent,
                    CreatedAt = DateTime.UtcNow,
                    SentAt = DateTime.UtcNow,
                    CreatedBy = "Diagnostics",
                    Category = "Test",
                    ViewCount = 0,
                    IsActive = true
                };

                await _messageSender.SendMessageAsync(testMessage);

                _logger.LogInformation("Test message sent to RabbitMQ successfully");

                return Ok(new
                {
                    success = true,
                    message = "Test message sent to RabbitMQ successfully!",
                    messageId = testMessage.Id,
                    title = testMessage.Title,
                    timestamp = DateTime.UtcNow,
                    instructions = "Check RabbitMQ Management UI at http://localhost:15672 to see the message in the 'broadcast-messages' queue"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send test message to RabbitMQ");

                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    type = ex.GetType().Name,
                    instructions = new[]
                    {
                        "1. Verify RabbitMQ is running: docker ps | findstr rabbitmq",
                        "2. Check appsettings.json RabbitMQ configuration",
                        "3. Verify connection settings (HostName, Port, Credentials)",
                        "4. Check application logs for detailed error information"
                    }
                });
            }
        }

        /// <summary>
        /// Get RabbitMQ configuration status
        /// GET: api/diagnostics/rabbitmq-config
        /// </summary>
        [HttpGet("rabbitmq-config")]
        public IActionResult GetRabbitMQConfig([FromServices] IConfiguration configuration)
        {
            var rabbitMqSection = configuration.GetSection("RabbitMQ");

            if (!rabbitMqSection.Exists())
            {
                return NotFound(new
                {
                    success = false,
                    message = "RabbitMQ configuration section not found in appsettings.json"
                });
            }

            return Ok(new
            {
                success = true,
                configuration = new
                {
                    hostName = rabbitMqSection["HostName"],
                    port = rabbitMqSection["Port"],
                    userName = rabbitMqSection["UserName"],
                    password = "***",  // Don't expose password
                    virtualHost = rabbitMqSection["VirtualHost"],
                    queueName = rabbitMqSection["QueueName"],
                    exchangeName = rabbitMqSection["ExchangeName"],
                    routingKey = rabbitMqSection["RoutingKey"]
                }
            });
        }

        /// <summary>
        /// Health check endpoint
        /// GET: api/diagnostics/health
        /// </summary>
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                application = "Broadcast Message System",
                version = "1.0",
                rabbitMqIntegration = "enabled"
            });
        }
    }
}
