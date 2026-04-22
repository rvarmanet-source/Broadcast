# 🎯 Broadcast Message Consumer

A standalone console application that listens to and displays broadcast messages from RabbitMQ in real-time.

## 📦 Overview

This console application acts as a consumer for the Broadcast Message System. It connects to RabbitMQ, listens for incoming broadcast messages, and displays them with rich formatting in the console.

## 🚀 Features

- ✅ Real-time message consumption from RabbitMQ
- ✅ Color-coded priority display (Urgent, High, Normal, Low)
- ✅ Beautiful console output with formatting
- ✅ Message metadata display (ID, Title, Content, Priority, etc.)
- ✅ RabbitMQ header information
- ✅ Graceful shutdown with Ctrl+C
- ✅ Error handling and troubleshooting guidance
- ✅ Configuration via appsettings.json

## 📋 Prerequisites

1. **.NET 10 SDK** installed
2. **RabbitMQ** running (Docker or standalone)
3. **Broadcast web application** configured and publishing messages

## ⚙️ Configuration

Edit `appsettings.json` to match your RabbitMQ setup:

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
    "RoutingKey": "broadcast.message"
  }
}
```

## 🏃 How to Run

### Option 1: Using dotnet CLI

```powershell
cd C:\Projects\Broadcast\BroadcastConsumer
dotnet run
```

### Option 2: Build and Run

```powershell
# Build the project
dotnet build

# Run the executable
cd bin\Debug\net10.0
.\BroadcastConsumer.exe
```

### Option 3: Visual Studio

1. Right-click on `BroadcastConsumer` project
2. Select "Set as Startup Project"
3. Press `F5` or click "Start"

## 📊 Usage

### Starting the Consumer

When you run the application, you'll see:

```
╔═══════════════════════════════════════════════════════════════╗
║       BROADCAST MESSAGE CONSUMER                              ║
║       RabbitMQ Message Queue Listener                         ║
╚═══════════════════════════════════════════════════════════════╝

Connected to: localhost:5672
Queue: broadcast-messages
Exchange: broadcast-exchange

Waiting for broadcast messages... (Press Ctrl+C to exit)
─────────────────────────────────────────────────────────────────
```

### Receiving Messages

When a message arrives, it's displayed with rich formatting:

```
┌─────────────────────────────────────────────────────────────┐
│ NEW BROADCAST MESSAGE RECEIVED - 2024-04-21 21:30:45       │
└─────────────────────────────────────────────────────────────┘
📨 Message ID: 1
📌 Title: System Maintenance Notice
⚡ Priority: 🔴 URGENT
📊 Status: Sent
🏷️  Category: Maintenance
─────────────────────────────────────────────────────────────
📄 Content:
The system will undergo maintenance tonight from 10 PM to 2 AM.
─────────────────────────────────────────────────────────────
👤 Created By: Admin
🕒 Created At: 2024-04-21 20:00:00
📤 Sent At: 2024-04-21 20:05:00
👁️  View Count: 5

📋 RabbitMQ Headers:
   priority: 4
   status: Sent
   category: Maintenance

✅ Message processed successfully
═════════════════════════════════════════════════════════════
```

### Stopping the Consumer

Press `Ctrl+C` to gracefully shut down the consumer.

## 🎨 Priority Color Coding

Messages are color-coded by priority:

- 🔴 **URGENT** - Red (Critical alerts)
- 🟠 **HIGH** - Yellow (Important announcements)
- 🔵 **NORMAL** - Blue (Regular updates)
- ⚫ **LOW** - Gray (Optional information)

## 🧪 Testing

### Test the Consumer

1. **Start RabbitMQ** (if not running):
   ```powershell
   docker start rabbitmq
   ```

2. **Run the Consumer**:
   ```powershell
   cd C:\Projects\Broadcast\BroadcastConsumer
   dotnet run
   ```

3. **Send a test message** from the web application:
   - Navigate to: http://localhost:5001/Broadcast
   - Create a message with "Send Immediately"
   - Watch it appear in the consumer console!

### Or use the diagnostic endpoint:

```powershell
# Send a test message via API
curl http://localhost:5001/api/diagnostics/test-rabbitmq
```

## 🏗️ Project Structure

```
BroadcastConsumer/
├── Configuration/
│   └── RabbitMQConfiguration.cs    # RabbitMQ settings
├── Models/
│   └── BroadcastMessage.cs         # Message model
├── Services/
│   └── MessageConsumer.cs          # Consumer logic
├── Program.cs                       # Entry point
├── appsettings.json                # Configuration
└── BroadcastConsumer.csproj        # Project file
```

## 🔧 Troubleshooting

### Connection Refused

**Error**: "Connection refused" or "Unable to connect"

**Solution**:
1. Verify RabbitMQ is running:
   ```powershell
   docker ps | findstr rabbitmq
   ```
2. Start RabbitMQ if needed:
   ```powershell
   docker start rabbitmq
   ```

### Authentication Failed

**Error**: "Authentication failed"

**Solution**: Check `appsettings.json` credentials (default: guest/guest)

### Queue Not Found

**Error**: "Queue 'broadcast-messages' not found"

**Solution**: 
1. Ensure the web application has run at least once
2. Check RabbitMQ Management UI: http://localhost:15672
3. Verify queue exists in the "Queues" tab

### No Messages Received

**Possible causes**:
1. Web application not sending messages
2. Wrong queue name in configuration
3. No messages in the queue

**Solution**:
1. Check RabbitMQ Management UI for message count
2. Send a test message from web app
3. Verify configuration matches web app settings

## 📚 Additional Resources

- **Main Documentation**: `../RABBITMQ-INTEGRATION.md`
- **Quick Start Guide**: `../RABBITMQ-QUICKSTART.md`
- **Web Application**: `../Broadcast/README.md`

## 🎯 Example Workflow

### Complete Test Scenario

1. **Start RabbitMQ**:
   ```powershell
   docker start rabbitmq
   ```

2. **Start Consumer** (Terminal 1):
   ```powershell
   cd C:\Projects\Broadcast\BroadcastConsumer
   dotnet run
   ```

3. **Start Web App** (Terminal 2):
   ```powershell
   cd C:\Projects\Broadcast\Broadcast
   dotnet run
   ```

4. **Send Message** (Browser):
   - Go to: http://localhost:5001/Broadcast
   - Create message with "Send Immediately"

5. **Watch Console** (Terminal 1):
   - See message appear instantly with formatting

6. **Stop Consumer**:
   - Press `Ctrl+C` in Terminal 1

## 🔍 Monitoring

### Check Messages in RabbitMQ

1. Open RabbitMQ Management UI: http://localhost:15672
2. Login: guest / guest
3. Go to "Queues" tab
4. Click on "broadcast-messages"
5. See message count and details

### View Message Details

In RabbitMQ UI:
1. Scroll to "Get messages" section
2. Set count to 1
3. Click "Get Message(s)"
4. View payload and headers

## 🚀 Running Multiple Consumers

You can run multiple instances of the consumer:

```powershell
# Terminal 1
cd C:\Projects\Broadcast\BroadcastConsumer
dotnet run

# Terminal 2
cd C:\Projects\Broadcast\BroadcastConsumer
dotnet run
```

Messages will be distributed among consumers using Round-Robin.

## 📝 Notes

- Consumer acknowledges messages after successful processing
- Failed messages are requeued automatically
- QoS set to process 1 message at a time
- Durable queue ensures messages survive restarts
- Graceful shutdown ensures no message loss

## ✅ Success Criteria

You know it's working when:

1. ✅ Consumer starts without errors
2. ✅ Shows "Waiting for broadcast messages..."
3. ✅ Displays connection details
4. ✅ Messages appear when sent from web app
5. ✅ Messages are formatted with colors
6. ✅ Ctrl+C shuts down gracefully

---

**Happy Message Consuming! 🎉**

*Last Updated: 2024*
