# 🐰 RabbitMQ Integration - Broadcast Message System

## Overview

The Broadcast Message System now integrates with **RabbitMQ** for reliable message queuing and delivery. Messages are automatically published to RabbitMQ when sent, enabling asynchronous processing, scalability, and fault tolerance.

---

## 🎯 Features

- ✅ **Automatic Message Publishing** - Messages sent to RabbitMQ when status changes to "Sent"
- ✅ **Durable Queues** - Messages persist across RabbitMQ restarts
- ✅ **Message Acknowledgment** - Ensures reliable message processing
- ✅ **Priority Headers** - Message priority included in headers
- ✅ **Configurable** - Easy configuration via appsettings.json
- ✅ **Background Consumer** - Optional consumer service for processing
- ✅ **Error Handling** - Comprehensive logging and error handling
- ✅ **Thread-Safe** - Singleton pattern for connection management

---

## 📦 Prerequisites

### 1. Install RabbitMQ

#### Windows (Using Chocolatey)
```powershell
# Install Chocolatey if not already installed
# Then install RabbitMQ
choco install rabbitmq

# Start RabbitMQ
rabbitmq-service start
```

#### Windows (Manual Installation)
1. Download and install [Erlang](https://www.erlang.org/downloads)
2. Download and install [RabbitMQ](https://www.rabbitmq.com/download.html)
3. Enable RabbitMQ Management Plugin:
```powershell
cd "C:\Program Files\RabbitMQ Server\rabbitmq_server-<version>\sbin"
rabbitmq-plugins enable rabbitmq_management
```

#### Docker
```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

#### Linux (Ubuntu/Debian)
```bash
sudo apt-get install rabbitmq-server
sudo systemctl enable rabbitmq-server
sudo systemctl start rabbitmq-server
sudo rabbitmq-plugins enable rabbitmq_management
```

### 2. Verify Installation

- RabbitMQ Server: `http://localhost:15672`
- Default credentials: `guest/guest`

---

## ⚙️ Configuration

### appsettings.json

The RabbitMQ configuration is located in `appsettings.json`:

```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "QueueName": "broadcast-messages",
    "ExchangeName": "broadcast-exchange",
    "RoutingKey": "broadcast.message",
    "DurableQueue": true,
    "AutoDeleteQueue": false
  }
}
```

### Configuration Options

| Setting | Description | Default |
|---------|-------------|---------|
| `HostName` | RabbitMQ server hostname | `localhost` |
| `Port` | RabbitMQ server port | `5672` |
| `UserName` | Authentication username | `guest` |
| `Password` | Authentication password | `guest` |
| `VirtualHost` | Virtual host | `/` |
| `QueueName` | Queue name for messages | `broadcast-messages` |
| `ExchangeName` | Exchange name | `broadcast-exchange` |
| `RoutingKey` | Routing key for messages | `broadcast.message` |
| `DurableQueue` | Queue survives broker restart | `true` |
| `AutoDeleteQueue` | Auto-delete queue when unused | `false` |

### Production Configuration

For production, update `appsettings.Production.json`:

```json
{
  "RabbitMQ": {
    "HostName": "your-rabbitmq-server.com",
    "Port": 5672,
    "UserName": "production-user",
    "Password": "strong-password",
    "VirtualHost": "/production",
    "QueueName": "broadcast-messages-prod",
    "ExchangeName": "broadcast-exchange-prod",
    "RoutingKey": "broadcast.message.prod",
    "DurableQueue": true,
    "AutoDeleteQueue": false
  }
}
```

---

## 🏗️ Architecture

### Message Flow

```
User Interface
      ↓
BroadcastController
      ↓
JsonBroadcastService
      ↓
BroadcastMessageSender → RabbitMQ Queue → BroadcastMessageConsumer
                              ↓
                        External Services
                        (Email, SMS, etc.)
```

### Components

#### 1. **BroadcastMessageSender** (Publisher)
- Publishes messages to RabbitMQ
- Singleton service
- Thread-safe connection management
- Automatic reconnection on failure

#### 2. **BroadcastMessageConsumer** (Consumer)
- Background service
- Listens for messages in queue
- Processes messages asynchronously
- Acknowledges successful processing
- Requeues failed messages

#### 3. **RabbitMQConfiguration**
- Configuration model
- Strongly-typed settings
- Easy to validate and test

---

## 🚀 Usage

### Publishing Messages

Messages are automatically published to RabbitMQ when:

1. **Creating a new message** with "Send Immediately" checked
2. **Editing a message** and marking it as "Send Immediately"
3. **Manually sending** a draft or scheduled message

```csharp
// In your controller or service
await _broadcastService.CreateMessageAsync(new BroadcastMessageViewModel
{
    Title = "Test Message",
    Content = "This will be sent to RabbitMQ",
    Priority = MessagePriority.High,
    SendImmediately = true
});
```

### Consuming Messages

#### Enable the Consumer

Uncomment this line in `Program.cs`:

```csharp
// Enable automatic message consumption
builder.Services.AddHostedService<BroadcastMessageConsumer>();
```

#### Custom Message Processing

Edit `BroadcastMessageConsumer.cs` in the `ProcessMessageAsync` method:

```csharp
private async Task ProcessMessageAsync(BroadcastMessage message)
{
    // Your custom logic here
    switch (message.Priority)
    {
        case MessagePriority.Urgent:
            await SendEmailNotification(message);
            await SendSMSNotification(message);
            break;
        case MessagePriority.High:
            await SendEmailNotification(message);
            break;
        default:
            await LogMessage(message);
            break;
    }
}
```

---

## 🔧 Advanced Features

### Message Headers

Each message includes these headers:

- `priority` - Message priority (1-4)
- `status` - Message status (Draft, Scheduled, Sent, Archived)
- `category` - Message category (optional)

### Message Properties

- **Persistent** - Messages survive broker restart
- **Content Type** - `application/json`
- **Message ID** - Unique message identifier
- **Timestamp** - Unix timestamp

### Quality of Service (QoS)

The consumer uses QoS settings:
- `prefetchCount: 1` - Process one message at a time
- Ensures fair distribution in multi-consumer scenarios

---

## 🔍 Monitoring

### RabbitMQ Management UI

Access the management UI at: `http://localhost:15672`

#### View Messages
1. Navigate to **Queues**
2. Click on `broadcast-messages`
3. See message count, rates, and details

#### Manual Testing
1. Go to **Queues** → `broadcast-messages`
2. Click **Get Messages**
3. View message payload and headers

### Application Logging

The application logs RabbitMQ operations:

```
[Information] Message published to RabbitMQ - ID: 1, Title: Test, Priority: High
[Information] Message processed and acknowledged - ID: 1
```

---

## 🐛 Troubleshooting

### Connection Issues

**Problem**: Cannot connect to RabbitMQ

**Solutions**:
1. Verify RabbitMQ is running:
   ```powershell
   rabbitmqctl status
   ```
2. Check firewall settings (port 5672)
3. Verify credentials in appsettings.json
4. Check RabbitMQ logs: `C:\Users\[User]\AppData\Roaming\RabbitMQ\log`

### Messages Not Being Consumed

**Problem**: Messages pile up in queue

**Solutions**:
1. Enable the consumer in `Program.cs`
2. Check consumer logs for errors
3. Verify queue bindings in RabbitMQ Management UI
4. Check for exceptions in message processing

### Message Serialization Errors

**Problem**: JSON deserialization fails

**Solutions**:
1. Verify message format matches `BroadcastMessage` model
2. Check for JSON syntax errors
3. Review serialization options

---

## 📊 Message Structure

### Published Message (JSON)

```json
{
  "Id": 1,
  "Title": "System Maintenance",
  "Content": "The system will be under maintenance tonight",
  "Priority": 3,
  "Status": 3,
  "CreatedAt": "2024-01-15T10:30:00Z",
  "ScheduledFor": null,
  "SentAt": "2024-01-15T10:30:00Z",
  "CreatedBy": "Admin",
  "Category": "Maintenance",
  "ViewCount": 0,
  "IsActive": true
}
```

### Message Headers

```json
{
  "priority": 3,
  "status": "Sent",
  "category": "Maintenance"
}
```

---

## 🔒 Security Best Practices

### Production Security

1. **Use Strong Credentials**
   ```json
   {
     "UserName": "prod-broadcast-user",
     "Password": "very-strong-password-here"
   }
   ```

2. **Use Environment Variables**
   ```csharp
   builder.Configuration["RabbitMQ:Password"] = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");
   ```

3. **Enable SSL/TLS**
   ```json
   {
     "Port": 5671,
     "UseSsl": true
   }
   ```

4. **Restrict Virtual Host Access**
   - Create dedicated virtual host
   - Grant minimal permissions

5. **Network Security**
   - Use firewall rules
   - Restrict access to internal network
   - Use VPN for external access

---

## 📈 Scaling Considerations

### Multiple Consumers

Run multiple instances of the consumer:

```csharp
// Each instance will process messages in parallel
builder.Services.AddHostedService<BroadcastMessageConsumer>();
```

### Clustering

For high availability, set up RabbitMQ cluster:
- Multiple RabbitMQ nodes
- Load balancer in front
- Shared message queues

### Message Throughput

Optimize for high throughput:
- Increase `prefetchCount`
- Use async processing
- Batch message publishing
- Monitor queue depth

---

## 🧪 Testing

### Manual Testing

1. **Start RabbitMQ**
2. **Run the application**
3. **Create a message** with "Send Immediately"
4. **Check RabbitMQ Management UI** - Message should appear in queue
5. **Enable consumer** - Message should be processed and removed

### Unit Testing

```csharp
[Fact]
public async Task SendMessageAsync_PublishesToRabbitMQ()
{
    // Arrange
    var config = Options.Create(new RabbitMQConfiguration());
    var logger = Mock.Of<ILogger<BroadcastMessageSender>>();
    var sender = new BroadcastMessageSender(config, logger);

    var message = new BroadcastMessage
    {
        Id = 1,
        Title = "Test",
        Content = "Test Content",
        Priority = MessagePriority.Normal
    };

    // Act
    await sender.SendMessageAsync(message);

    // Assert - Check RabbitMQ queue
}
```

---

## 📚 Additional Resources

- [RabbitMQ Documentation](https://www.rabbitmq.com/documentation.html)
- [RabbitMQ .NET Client Guide](https://www.rabbitmq.com/dotnet-api-guide.html)
- [RabbitMQ Best Practices](https://www.rabbitmq.com/best-practices.html)
- [RabbitMQ Monitoring](https://www.rabbitmq.com/monitoring.html)

---

## 🎯 Next Steps

1. ✅ Install RabbitMQ
2. ✅ Configure appsettings.json
3. ✅ Run the application
4. ✅ Create and send a test message
5. ✅ Monitor in RabbitMQ Management UI
6. 🔜 Implement custom message processing
7. 🔜 Enable consumer service
8. 🔜 Set up production configuration
9. 🔜 Implement monitoring and alerting

---

**Happy Messaging! 🐰🚀**

*Last Updated: 2024*
