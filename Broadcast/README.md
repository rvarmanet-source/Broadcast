# 🎯 Broadcast Message System

> A complete ASP.NET Core 10 MVC application with a powerful Broadcast Message System using JSON as a database.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?style=flat-square)](https://asp.net/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?style=flat-square&logo=bootstrap&logoColor=white)](https://getbootstrap.com/)
[![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)](LICENSE)
[![Status](https://img.shields.io/badge/Status-Production%20Ready-brightgreen?style=flat-square)](https://github.com)

---

## 📖 Overview

Broadcast is a modern web application that enables you to create, manage, schedule, and send broadcast messages to users. Built with **ASP.NET Core 10** and using **JSON files** as a lightweight database, it's perfect for teams and organizations that need a simple yet powerful messaging system without the complexity of traditional databases.

### ✨ Key Features

- 📝 **Full CRUD Operations** - Create, Read, Update, Delete messages
- 🎚️ **Priority Levels** - Urgent, High, Normal, Low
- 📊 **Status Tracking** - Draft, Scheduled, Sent, Archived
- ⏰ **Message Scheduling** - Send now or schedule for later
- 👁️ **Analytics** - View count tracking
- 🎨 **Beautiful UI** - Bootstrap 5 with responsive design
- 🐰 **RabbitMQ Integration** - Reliable message queuing ⭐ NEW
- 🚀 **Zero Dependencies** - No SQL Server, No Entity Framework
- 💾 **JSON Storage** - Portable, human-readable data
- 🔒 **Secure** - CSRF protection, input validation, HTTPS

---

## 🚀 Quick Start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio 2026 or VS Code (optional)

### Installation & Run

```bash
# Clone the repository
git clone https://github.com/rvarmanet-source/Broadcast.git

# Navigate to the project
cd Broadcast/Broadcast

# Run the application
dotnet run
```

Open your browser to: `https://localhost:5001`

That's it! 🎉

---

## 📸 Screenshots

### All Messages View
![All Messages](docs/images/all-messages.png)
*Manage all broadcast messages with table view*

### Active Messages View
![Active Messages](docs/images/active-messages.png)
*Beautiful card-based display of active messages*

### Create Message Form
![Create Form](docs/images/create-message.png)
*Easy-to-use form with validation*

---

## 🎯 Usage

### Create a Broadcast Message

1. Navigate to **Messages** in the navigation bar
2. Click **"Create New Message"**
3. Fill in the form:
   - **Title**: Short, descriptive title
   - **Content**: Your message (up to 2000 characters)
   - **Priority**: Choose urgency level
   - **Category**: Optional categorization
   - **Send Immediately**: Toggle for instant or scheduled delivery
4. Click **"Create Message"**

### View Active Messages

- Click **"Active Messages"** to see all currently active broadcasts
- Messages are displayed in priority order
- View counts are tracked automatically

### Manage Messages

- **Edit**: Click the pencil icon to modify
- **Send**: Click the paper plane to send drafts
- **Archive**: Click the trash icon to remove

---

## 🏗️ Architecture

```
┌─────────────────────────────────────┐
│          User Interface             │
│  (Bootstrap 5 + Razor Views)        │
└─────────────┬───────────────────────┘
              │
              ↓
┌─────────────────────────────────────┐
│     Broadcast Controller            │
│     (MVC Pattern)                   │
└─────────────┬───────────────────────┘
              │
              ↓
┌─────────────────────────────────────┐
│   IBroadcastService Interface       │
│   (Service Layer)                   │
└─────────────┬───────────────────────┘
              │
              ↓
┌─────────────────────────────────────┐
│   JsonBroadcastService              │
│   (JSON Implementation)             │
└─────────────┬───────────────────────┘
              │
              ↓
┌─────────────────────────────────────┐
│   broadcast-messages.json           │
│   (Data Storage)                    │
└─────────────────────────────────────┘
```

**[See complete architecture diagrams →](ARCHITECTURE.md)**

---

## 📂 Project Structure

```
Broadcast/
├── Controllers/
│   ├── HomeController.cs
│   └── BroadcastController.cs         # Message management
├── Models/
│   ├── BroadcastMessage.cs            # Entity model
│   └── BroadcastMessageViewModel.cs   # Form model
├── Services/
│   ├── IBroadcastService.cs
│   ├── JsonBroadcastService.cs        # JSON storage
│   ├── IBroadcastMessageSender.cs     # RabbitMQ interface ⭐
│   ├── BroadcastMessageSender.cs      # RabbitMQ publisher ⭐
│   ├── BroadcastMessageConsumer.cs    # RabbitMQ consumer ⭐
│   └── RabbitMQConfiguration.cs       # RabbitMQ config ⭐
├── Views/
│   ├── Broadcast/
│   │   ├── Index.cshtml               # All messages
│   │   ├── Active.cshtml              # Active messages
│   │   ├── Create.cshtml              # Create form
│   │   ├── Edit.cshtml                # Edit form
│   │   └── Details.cshtml             # Message details
│   └── Shared/
│       └── _Layout.cshtml             # Master layout
├── Data/
│   └── broadcast-messages.json        # JSON database
└── wwwroot/                           # Static assets
```

---

## 🐰 RabbitMQ Integration

### What's New

Messages are now automatically published to **RabbitMQ** when sent, enabling:
- ✅ Asynchronous message processing
- ✅ Scalable architecture
- ✅ Reliable message delivery
- ✅ Fault tolerance
- ✅ Easy integration with external services

### Quick Setup

1. **Install RabbitMQ** (Docker - easiest):
   ```bash
   docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
   ```

2. **Verify Installation**:
   - Open: http://localhost:15672
   - Login: guest / guest

3. **Run Your App** - RabbitMQ integration is automatic!

4. **Test**: Create a message with "Send Immediately"

5. **Monitor**: Check RabbitMQ Management UI

### Documentation

| Document | Description |
|----------|-------------|
| **[RABBITMQ-INTEGRATION.md](RABBITMQ-INTEGRATION.md)** | Complete RabbitMQ guide |
| **[RABBITMQ-QUICKSTART.md](RABBITMQ-QUICKSTART.md)** | Quick reference card |

**[🐰 Read RabbitMQ Integration Guide →](RABBITMQ-INTEGRATION.md)**

---

## 💻 Technology Stack

### Backend
- **Framework**: ASP.NET Core 10.0
- **Language**: C# 13
- **Pattern**: MVC (Model-View-Controller)
- **Storage**: JSON Files
- **Message Queue**: RabbitMQ ⭐
- **Features**: Async/Await, Dependency Injection, Logging

### Frontend
- **UI Framework**: Bootstrap 5
- **Icons**: Bootstrap Icons
- **JavaScript**: jQuery
- **Validation**: jQuery Validation
- **Design**: Responsive, Mobile-First

---

## 📚 Documentation

Comprehensive documentation is available:

| Document | Description |
|----------|-------------|
| **[QUICK-START.md](QUICK-START.md)** | Get up and running in 5 minutes |
| **[BROADCAST-README.md](BROADCAST-README.md)** | Complete implementation guide |
| **[ARCHITECTURE.md](ARCHITECTURE.md)** | Technical architecture & diagrams |
| **[FEATURES.md](FEATURES.md)** | Full feature documentation |
| **[CHECKLIST.md](CHECKLIST.md)** | Implementation status & roadmap |
| **[RABBITMQ-INTEGRATION.md](RABBITMQ-INTEGRATION.md)** | RabbitMQ integration guide ⭐ |
| **[RABBITMQ-QUICKSTART.md](RABBITMQ-QUICKSTART.md)** | RabbitMQ quick reference ⭐ |
| **[README-INDEX.md](README-INDEX.md)** | Documentation navigation |

**[📖 View complete documentation index →](README-INDEX.md)**

---

## 🎨 Features in Detail

### Message Management
- ✅ Create messages with rich metadata
- ✅ Edit messages before or after sending
- ✅ Archive old messages
- ✅ View detailed message information
- ✅ Track message views and engagement

### Priority System
- 🔴 **Urgent** - Critical alerts
- 🟠 **High** - Important announcements
- 🔵 **Normal** - Regular updates
- ⚫ **Low** - Optional information

### Status Workflow
```
Draft → Scheduled → Sent → Archived
```

### User Interface
- 📊 Table view for management
- 🎴 Card view for display
- 🎨 Color-coded priorities
- 📱 Responsive design
- ✨ Bootstrap Icons
- 🔔 Success/error notifications

---

## 🚀 Performance

- **Suitable for**: Small to medium deployments (< 10,000 messages)
- **Read operations**: Fast JSON parsing
- **Write operations**: Thread-safe file updates
- **Concurrency**: SemaphoreSlim for thread safety
- **Memory**: Efficient async operations

---

## 🔒 Security

- ✅ CSRF Protection (Anti-Forgery Tokens)
- ✅ Input Validation (Data Annotations)
- ✅ HTTPS Redirection
- ✅ HSTS in Production
- ✅ Secure File Operations
- ✅ No SQL Injection (No SQL!)

---

## 🔄 Future Enhancements

### Planned Features (Phase 2)
- 🔐 User authentication (ASP.NET Identity)
- 👥 Role-based authorization
- 📧 Email notifications
- 📱 Push notifications
- 🔍 Advanced search & filtering
- 📄 Pagination
- 📊 Analytics dashboard
- 📤 Export functionality

### Enterprise Features (Phase 3)
- 🗄️ SQL Server migration option
- 🎯 Audience segmentation
- 🔄 Message templates
- 📅 Recurring messages
- 🌐 Multi-language support
- 📈 A/B testing
- 🔌 REST API
- 📱 Mobile app

**[See complete roadmap →](CHECKLIST.md#-phase-2-enhancements-future)**

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👥 Authors

- **Development Team** - *Initial work* - [rvarmanet-source](https://github.com/rvarmanet-source)

---

## 🙏 Acknowledgments

- ASP.NET Core team for the amazing framework
- Bootstrap team for the UI framework
- Bootstrap Icons for the beautiful icons
- Community for inspiration and support

---

## 📞 Support

- 📧 Email: support@example.com
- 🐛 Issues: [GitHub Issues](https://github.com/rvarmanet-source/Broadcast/issues)
- 💬 Discussions: [GitHub Discussions](https://github.com/rvarmanet-source/Broadcast/discussions)
- 📖 Documentation: [README-INDEX.md](README-INDEX.md)

---

## 🌟 Show Your Support

Give a ⭐️ if this project helped you!

---

## 📊 Project Stats

- **Version**: 1.0
- **Status**: Production Ready ✅
- **Build**: Passing ✅
- **Documentation**: Complete ✅
- **Code Quality**: Excellent ✅

---

## 🎉 Get Started Now!

```bash
dotnet run
```

Then open `https://localhost:5001` and start broadcasting! 🚀

---

*Built with ❤️ using ASP.NET Core 10*

*Last Updated: 2024*
