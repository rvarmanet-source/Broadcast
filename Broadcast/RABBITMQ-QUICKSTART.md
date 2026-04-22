# 🎯 RabbitMQ Quick Reference Card

## 🚀 Quick Start

### 1. Install RabbitMQ (Docker - Easiest)
```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

### 2. Verify Installation
- Management UI: http://localhost:15672
- Login: guest / guest

### 3. Run Your Application
```bash
dotnet run
```

### 4. Send a Test Message
1. Navigate to: http://localhost:5001/Broadcast
2. Click "Create New Message"
3. Fill form and check "Send Immediately"
4. Click "Create Message"

### 5. Monitor Messages
- Go to: http://localhost:15672
- Click "Queues" → "broadcast-messages"
- See your message!

---

## 📂 Files Created

| File | Purpose |
|------|---------|
| `BroadcastMessageSender.cs` | ✅ Publisher - Sends messages to RabbitMQ |
| `BroadcastMessageConsumer.cs` | ✅ Consumer - Processes messages from queue |
| `RabbitMQConfiguration.cs` | ✅ Configuration model |
| `IBroadcastMessageSender.cs` | ✅ Service interface |
| `RABBITMQ-INTEGRATION.md` | ✅ Complete documentation |

---

## ⚙️ Configuration (appsettings.json)

```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "QueueName": "broadcast-messages",
    "ExchangeName": "broadcast-exchange",
    "RoutingKey": "broadcast.message"
  }
}
```

---

## 🔄 How It Works

```
User creates message
        ↓
JsonBroadcastService (saves to JSON)
        ↓
BroadcastMessageSender (publishes to RabbitMQ)
        ↓
RabbitMQ Queue (stores message)
        ↓
BroadcastMessageConsumer (optional - processes message)
        ↓
Your custom logic (email, SMS, etc.)
```

---

## 📝 Enable Consumer (Optional)

**In Program.cs**, uncomment:

```csharp
builder.Services.AddHostedService<BroadcastMessageConsumer>();
```

This will automatically process messages from the queue.

---

## 🧪 Testing Commands

### Check RabbitMQ Status
```powershell
# Windows
rabbitmqctl status

# Docker
docker exec rabbitmq rabbitmqctl status
```

### View Queues
```powershell
# Windows
rabbitmqctl list_queues

# Docker
docker exec rabbitmq rabbitmqctl list_queues
```

### Purge Queue (Clear all messages)
```powershell
# Windows
rabbitmqctl purge_queue broadcast-messages

# Docker
docker exec rabbitmq rabbitmqctl purge_queue broadcast-messages
```

---

## 🎨 Message Format

### JSON Payload
```json
{
  "Id": 1,
  "Title": "Test Message",
  "Content": "Message content here",
  "Priority": 3,
  "Status": 3,
  "CreatedAt": "2024-01-15T10:30:00Z",
  "SentAt": "2024-01-15T10:30:00Z",
  "CreatedBy": "Admin",
  "Category": "Test",
  "IsActive": true
}
```

### Message Headers
- `priority`: 1-4 (Low, Normal, High, Urgent)
- `status`: Draft/Scheduled/Sent/Archived
- `category`: Custom category

---

## 🐛 Common Issues

### "Connection refused"
- ✅ **Solution**: Start RabbitMQ service
  ```bash
  # Docker
  docker start rabbitmq

  # Windows Service
  rabbitmq-service start
  ```

### "Authentication failed"
- ✅ **Solution**: Check username/password in appsettings.json
- Default is: guest/guest

### Messages not being consumed
- ✅ **Solution**: Enable consumer in Program.cs
- ✅ Check consumer logs for errors

---

## 📊 RabbitMQ Management UI

### Access
http://localhost:15672

### Useful Pages
- **Overview**: System status
- **Queues**: View queues and messages
- **Exchanges**: View exchanges and bindings
- **Connections**: Active connections

### Manual Testing
1. Go to **Queues** → `broadcast-messages`
2. Click **Get Messages**
3. Set count to 1
4. Click **Get Message(s)**
5. View message payload

---

## 🔑 Key Code Locations

### Send Message
**File**: `JsonBroadcastService.cs`
```csharp
// Automatically sends when status is "Sent"
await _messageSender.SendMessageAsync(message);
```

### Process Message
**File**: `BroadcastMessageConsumer.cs`
```csharp
private async Task ProcessMessageAsync(BroadcastMessage message)
{
    // Your custom logic here
    _logger.LogInformation("Processing: {Title}", message.Title);
}
```

---

## 📈 Monitoring

### Log Messages to Watch For

**Success**:
```
[Information] Message published to RabbitMQ - ID: 1, Title: Test
[Information] Message processed and acknowledged - ID: 1
```

**Errors**:
```
[Error] Failed to publish message to RabbitMQ - ID: 1
[Error] Failed to initialize RabbitMQ connection
```

---

## 🎯 Next Actions

1. ✅ RabbitMQ installed and running
2. ✅ Configuration updated
3. ✅ Code integrated
4. ✅ Build successful
5. 🔜 **Test**: Create and send a message
6. 🔜 **Monitor**: Check RabbitMQ Management UI
7. 🔜 **Customize**: Add your processing logic
8. 🔜 **Deploy**: Configure for production

---

## 📞 Quick Links

- **Full Documentation**: [RABBITMQ-INTEGRATION.md](RABBITMQ-INTEGRATION.md)
- **RabbitMQ Docs**: https://www.rabbitmq.com/documentation.html
- **Management UI**: http://localhost:15672
- **Application**: http://localhost:5001/Broadcast

---

## 💡 Pro Tips

1. **Always monitor the queue** in RabbitMQ UI during testing
2. **Enable logging** to see message flow
3. **Start with consumer disabled** to see messages pile up
4. **Use priority** for important messages
5. **Check acknowledgment** logs to ensure processing

---

**You're all set! 🎉**

Create a message with "Send Immediately" and watch it flow through RabbitMQ! 🐰✨
