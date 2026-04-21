# Broadcast Message System - Implementation Guide

## 🎯 Overview
    
A complete **Broadcast Message System** implemented in ASP.NET Core 10 using **JSON as a database**. This lightweight solution allows you to create, manage, schedule, and send broadcast messages without requiring SQL Server or Entity Framework.

---

## 📦 Features Implemented

### ✅ **Core Functionality**
- ✨ **Create** broadcast messages with title, content, priority, and category
- ✏️ **Edit** existing messages
- 👀 **View** all messages with filtering (All/Active)
- 🗑️ **Delete** (Archive) messages
- 📤 **Send** messages immediately or schedule for later
- 🎚️ **Priority levels**: Urgent, High, Normal, Low
- 📊 **Message status tracking**: Draft, Scheduled, Sent, Archived
- 👁️ **View count** tracking for analytics
- 🎨 **Rich UI** with Bootstrap 5 and Bootstrap Icons

### 🏗️ **Architecture**

```
Broadcast/
├── Controllers/
│   └── BroadcastController.cs       # Handles all CRUD operations
├── Models/
│   ├── BroadcastMessage.cs          # Main message model
│   └── BroadcastMessageViewModel.cs # ViewModel for forms
├── Services/
│   ├── IBroadcastService.cs         # Service interface
│   └── JsonBroadcastService.cs      # JSON-based implementation
├── Views/Broadcast/
│   ├── Index.cshtml                 # List all messages
│   ├── Active.cshtml                # Show active messages
│   ├── Create.cshtml                # Create new message
│   ├── Edit.cshtml                  # Edit message
│   └── Details.cshtml               # View message details
└── Data/
    └── broadcast-messages.json      # JSON database file
```

---

## 🚀 Getting Started

### 1. **Prerequisites**
- .NET 10 SDK installed
- Visual Studio 2026 or VS Code
- No database setup required!

### 2. **Run the Application**

```bash
cd C:\Projects\Broadcast\Broadcast
dotnet run
```

Or press **F5** in Visual Studio

### 3. **Access the Application**

Navigate to: `https://localhost:5001` (or the port shown in console)

### 4. **Navigate to Broadcast Messages**

Click on **Messages** in the navigation menu or go directly to:
- All Messages: `https://localhost:5001/Broadcast`
- Active Messages: `https://localhost:5001/Broadcast/Active`

---

## 📝 How to Use

### **Creating a Message**

1. Click **"Create New Message"** button
2. Fill in the form:
   - **Title**: Short, descriptive title (max 200 chars)
   - **Content**: Your message content (max 2000 chars)
   - **Priority**: Low, Normal, High, or Urgent
   - **Category**: Optional category (e.g., "Announcement", "Alert")
   - **Send Immediately**: Toggle to send now or schedule for later
   - **Schedule For**: Set future date/time if not sending immediately
3. Click **"Create Message"**

### **Viewing Messages**

- **All Messages View**: See all messages with full details in a table
- **Active Messages View**: Card-based view of currently active messages
- **Details View**: Click the eye icon to see full message details and statistics

### **Editing a Message**

1. Click the pencil icon on any message
2. Modify the fields as needed
3. Click **"Save Changes"**

### **Sending a Message**

- For Draft/Scheduled messages, click the **"Send Now"** button (paper plane icon)
- Confirm the action
- Message status will change to "Sent" with timestamp

### **Archiving a Message**

- Click the trash icon on any message
- Confirm the action
- Message will be marked as inactive/archived

---

## 🎨 UI Features

### **Priority-Based Styling**
- **Urgent**: Red badge, special highlighting
- **High**: Orange/warning badge
- **Normal**: Blue/info badge
- **Low**: Gray/secondary badge

### **Status Indicators**
- **Sent**: Green badge
- **Scheduled**: Blue badge
- **Draft**: Gray badge
- **Archived**: Dark badge

### **Responsive Design**
- Mobile-friendly navigation
- Collapsible navbar
- Card-based layouts on mobile
- Table layouts on desktop

---

## 💾 Data Storage

### **JSON Database**

Messages are stored in: `Broadcast\Data\broadcast-messages.json`

**Sample Structure:**
```json
[
  {
    "Id": 1,
    "Title": "Welcome Message",
    "Content": "Welcome to the Broadcast System!",
    "Priority": 2,
    "Status": 3,
    "CreatedAt": "2024-01-15T10:30:00Z",
    "ScheduledFor": null,
    "SentAt": "2024-01-15T10:30:00Z",
    "CreatedBy": "Admin",
    "Category": "Announcement",
    "ViewCount": 5,
    "IsActive": true
  }
]
```

### **Automatic Initialization**

- File is created automatically on first run
- Includes a welcome message as sample data
- Thread-safe read/write operations

### **Data Backup**

Simply copy the `broadcast-messages.json` file to backup your data:

```bash
copy Data\broadcast-messages.json Data\broadcast-messages-backup.json
```

---

## 🔧 Configuration

### **No Configuration Required!**

The system works out-of-the-box with sensible defaults:
- JSON file stored in `Data` folder
- Automatic file creation
- No connection strings needed
- No database migrations

---

## 🎯 Priority Levels

| Level | Value | Use Case |
|-------|-------|----------|
| **Urgent** | 4 | Critical alerts requiring immediate attention |
| **High** | 3 | Important announcements |
| **Normal** | 2 | Regular updates and information |
| **Low** | 1 | Optional information |

---

## 📊 Message Status Flow

```
Draft → Scheduled → Sent → Archived
  ↓         ↓         ↓
  └─────────┴─────────→ Sent (via "Send Now")
```

1. **Draft**: Saved but not scheduled
2. **Scheduled**: Set for future delivery
3. **Sent**: Active and visible to users
4. **Archived**: Soft-deleted (IsActive = false)

---

## 🛠️ Technical Details

### **Service Layer**

**Interface**: `IBroadcastService`
```csharp
- GetAllMessagesAsync()
- GetActiveMessagesAsync()
- GetMessageByIdAsync(int id)
- CreateMessageAsync(BroadcastMessageViewModel model)
- UpdateMessageAsync(int id, BroadcastMessageViewModel model)
- DeleteMessageAsync(int id)
- SendMessageAsync(int id)
- GetActiveMessageCountAsync()
- IncrementViewCountAsync(int id)
```

**Implementation**: `JsonBroadcastService`
- Thread-safe file operations using `SemaphoreSlim`
- Automatic ID generation
- JSON serialization with pretty-print formatting
- Logging integration

### **Controller Actions**

| Action | Method | Description |
|--------|--------|-------------|
| `Index` | GET | List all messages |
| `Active` | GET | List active messages |
| `Details` | GET | View message details |
| `Create` | GET/POST | Create new message |
| `Edit` | GET/POST | Edit existing message |
| `Delete` | POST | Archive message |
| `Send` | POST | Send message immediately |

---

## 🎨 Customization

### **Adding New Categories**

Categories are free-form text. Common examples:
- Announcement
- Alert
- Update
- Maintenance
- Information

### **Modifying Priority Colors**

Edit the views to change badge colors:
```html
<!-- In Index.cshtml, Active.cshtml, Details.cshtml -->
<span class="badge bg-danger">Urgent</span>
<span class="badge bg-warning">High</span>
<span class="badge bg-info">Normal</span>
<span class="badge bg-secondary">Low</span>
```

### **Changing Storage Location**

Modify `JsonBroadcastService.cs` constructor:
```csharp
_jsonFilePath = Path.Combine(environment.ContentRootPath, "YourFolder", "your-file.json");
```

---

## 📈 Future Enhancements

Consider adding these features:

### **Phase 2 Enhancements**
- 🔔 Push notifications
- 👥 User targeting (send to specific groups)
- 📧 Email integration
- 📱 SMS integration
- 🔍 Search and filtering
- 📤 Export messages (CSV, PDF)
- 📊 Analytics dashboard
- 🔒 User authentication
- 👮 Role-based permissions

### **Phase 3 Enhancements**
- 📅 Recurring messages
- 🎨 Rich text editor
- 📎 File attachments
- 🌐 Multi-language support
- 📈 Delivery statistics
- 🎯 A/B testing
- 🤖 Automated scheduling
- 🔄 Message templates

---

## 🐛 Troubleshooting

### **Issue**: JSON file not found

**Solution**: File is created automatically. Check permissions on the `Data` folder.

### **Issue**: Changes not saving

**Solution**: Ensure the application has write permissions to the `Data` folder.

### **Issue**: Build errors

**Solution**: Run `dotnet clean` and `dotnet build` in the project folder.

### **Issue**: Icons not showing

**Solution**: Check internet connection (Bootstrap Icons loaded from CDN).

---

## 📚 Navigation

### **Menu Structure**

```
Home
  └── Home page with welcome message
Messages
  └── All broadcast messages (management)
Active Messages
  └── Currently active messages (public view)
Privacy
  └── Privacy policy page
```

---

## 💡 Best Practices

1. **Message Content**
   - Keep titles under 50 characters for better display
   - Use clear, concise language
   - Proofread before sending

2. **Priority Usage**
   - Reserve "Urgent" for critical issues only
   - Use "Normal" for most messages
   - Use "Low" for optional information

3. **Scheduling**
   - Schedule messages during business hours
   - Consider time zones if applicable
   - Test with draft messages first

4. **Categories**
   - Be consistent with category names
   - Use title case (e.g., "System Update")
   - Limit to 3-5 common categories

5. **Archiving**
   - Archive old messages regularly
   - Keep active messages relevant
   - Review and clean up monthly

---

## 🔒 Security Considerations

### **Current Implementation**
- No authentication (suitable for internal use)
- No authorization checks
- All users can create/edit messages

### **For Production Use**, Consider Adding:
1. **Authentication**: ASP.NET Core Identity
2. **Authorization**: Role-based access control
3. **Input validation**: XSS protection
4. **Rate limiting**: Prevent abuse
5. **Audit logging**: Track who did what

---

## 📄 License

This is a sample implementation for educational purposes.

---

## 🙋 Support

For questions or issues:
1. Check the **Troubleshooting** section above
2. Review the code comments in the files
3. Check ASP.NET Core documentation

---

## 🎉 Quick Start Checklist

- [x] Project created with .NET 10
- [x] JSON service implemented
- [x] Controllers and views created
- [x] Navigation menu updated
- [x] Bootstrap icons configured
- [x] Sample data included
- [x] Build successful
- [ ] **Run the application** (`dotnet run` or F5)
- [ ] **Create your first message**
- [ ] **Explore the features**

---

**Enjoy your Broadcast Message System! 🚀**
