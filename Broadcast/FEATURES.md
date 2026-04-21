# Broadcast - Feature Documentation

## Project Overview
**Broadcast** is an ASP.NET Core 10.0 MVC web application with a comprehensive Broadcast Message System. The application uses JSON as a database for lightweight, portable data storage without requiring SQL Server or Entity Framewor	k.

---

## 📋 Table of Contents
- [Technology Stack](#technology-stack)
- [Core Features](#core-features)
- [Broadcast Message System](#broadcast-message-system)
- [Application Architecture](#application-architecture)
- [Frontend Features](#frontend-features)
- [Backend Features](#backend-features)
- [Configuration & Settings](#configuration--settings)
- [Project Structure](#project-structure)

---

## 🛠 Technology Stack

### Backend
- **.NET 10.0** - Latest .NET framework
- **ASP.NET Core MVC** - Model-View-Controller pattern
- **C# 13** - Programming language with nullable reference types enabled
- **Implicit Usings** - Simplified using declarations
- **System.Text.Json** - JSON serialization/deserialization

### Frontend
- **Bootstrap 5** - Responsive CSS framework
- **Bootstrap Icons** - Icon library for UI elements
- **jQuery** - JavaScript library
- **jQuery Validation** - Form validation
- **Razor Views** - Server-side rendering engine

### Data Storage
- **JSON Files** - Lightweight, portable database
- **No SQL Server Required** - File-based storage
- **No Entity Framework** - Simple JSON serialization

---

## 🎯 Core Features

### 1. **Broadcast Message System** ⭐ NEW
Complete message management system with the following capabilities:

#### **Message Management**
- ✅ **Create** broadcast messages with title, content, priority, and category
- ✅ **Edit** existing messages before or after sending
- ✅ **View** all messages with filtering options
- ✅ **Delete** (Archive) messages with soft-delete functionality
- ✅ **Send** messages immediately or schedule for later
- ✅ **Search & Filter** by status (All/Active messages)

#### **Message Properties**
- **Priority Levels**: Urgent, High, Normal, Low
- **Status Tracking**: Draft, Scheduled, Sent, Archived
- **Categories**: Custom categorization (Announcement, Alert, etc.)
- **Scheduling**: Future delivery with date/time picker
- **Analytics**: View count tracking per message
- **Timestamps**: Created, Scheduled, and Sent timestamps

#### **User Interface**
- Table view for message management
- Card view for active messages display
- Responsive design for mobile and desktop
- Priority-based color coding
- Status badges and indicators
- Bootstrap Icons integration
- Form validation with client and server-side checks
- Success/Error notifications using TempData
- Confirmation dialogs for destructive actions

### 2. **Web Application Framework**
- **MVC Architecture**: Clean separation of concerns using Model-View-Controller pattern
- **Routing**: Conventional routing with default route pattern `{controller=Home}/{action=Index}/{id?}`
- **Static Asset Optimization**: Built-in static asset mapping for improved performance
- **HTTPS Redirection**: Automatic HTTPS enforcement for secure connections

### 3. **Security Features**
- **HSTS (HTTP Strict Transport Security)**
  - Enabled in production environments
  - Default: 30-day max-age
  - Forces secure HTTPS connections
- **Anti-Forgery Tokens**: CSRF protection on all forms
- **Input Validation**: Server-side validation with data annotations

### 4. **Error Handling**
- **Custom Error Pages**
  - Development vs. Production error handling
  - User-friendly error views in production
  - Detailed error information in development

- **Error Tracking**
  - Request ID tracking using Activity.Current.Id
  - HttpContext TraceIdentifier for correlation
  - Response caching disabled for error pages

- **Broadcast message**
	- Allow user can give the input and broadcast using send feature
	- 

## 🏗 Application Architecture

### Controllers

#### **HomeController**
Primary controller managing the main application pages.

**Actions:**
- `Index()` - Home page/landing page
- `Privacy()` - Privacy policy page
- `Error()` - Error page with diagnostic information

**Features:**
- Response caching configuration for error action
- Activity and trace identifier tracking

### Views

#### **Home Views**
1. **Index.cshtml**
   - Welcome page
   - Links to ASP.NET Core documentation
   - Centered text layout

2. **Privacy.cshtml**
   - Privacy policy information page
   - Customizable privacy content

#### **Shared Views**
1. **_Layout.cshtml**
   - Master layout template
   - Responsive navigation bar
   - Bootstrap-based design
   - Footer with copyright information
   - Script and CSS bundle management

2. **Error.cshtml**
   - Error display page
   - Shows request ID for troubleshooting

3. **_ValidationScriptsPartial.cshtml**
   - Client-side validation scripts
   - jQuery validation library integration

4. **_ViewImports.cshtml**
   - Global view imports
   - Tag helpers and namespaces

5. **_ViewStart.cshtml**
   - Sets default layout for all views

#### **Broadcast Views** ⭐ NEW
1. **Index.cshtml**
   - List all broadcast messages in table format
   - Action buttons for edit, send, delete
   - Priority and status badges
   - Search and filter options

2. **Active.cshtml**
   - Card-based display of active messages
   - Priority-based styling
   - View count display
   - Category badges

3. **Create.cshtml**
   - Form to create new messages
   - Priority selection dropdown
   - Category input field
   - Send immediately or schedule toggle
   - Date/time picker for scheduling
   - Client-side validation

4. **Edit.cshtml**
   - Edit existing message form
   - Pre-populated fields
   - Same validation as Create
   - Ability to change status

5. **Details.cshtml**
   - Full message details display
   - Message metadata (created, sent, views)
   - Priority and status indicators
   - Edit and Send action buttons

### Models

#### **ErrorViewModel**
- Contains error information
- Tracks RequestId for debugging
- Includes ShowRequestId property

#### **BroadcastMessage** ⭐ NEW
- Main message entity with properties:
  - Id, Title, Content
  - Priority (enum: Low, Normal, High, Urgent)
  - Status (enum: Draft, Scheduled, Sent, Archived)
  - CreatedAt, ScheduledFor, SentAt
  - CreatedBy, Category
  - ViewCount, IsActive

#### **BroadcastMessageViewModel** ⭐ NEW
- Form view model for creating/editing
- Data annotations for validation
- Display attributes for labels
- Optimized for form binding

---

## 🎨 Frontend Features

### Responsive Design
- **Mobile-First Approach**: Bootstrap 5 grid system
- **Responsive Navigation**: Collapsible navbar for mobile devices
- **Viewport Optimization**: Proper meta viewport configuration
- **Card Layouts**: Mobile-optimized for active messages
- **Table Layouts**: Desktop-optimized for message management

### UI Components
- **Navigation Bar**
  - Brand logo link
  - Responsive toggle button
  - Home, Messages, Active Messages, and Privacy links
  - Clean, modern design

- **Layout System**
  - Container-based layout
  - Main content area with proper spacing
  - Sticky footer design

- **Broadcast UI Elements** ⭐ NEW
  - Priority badges (color-coded)
  - Status indicators
  - Action button groups
  - Confirmation dialogs
  - Alert notifications
  - Form validation feedback
  - Bootstrap Icons throughout

### Static Assets
- **CSS Libraries**
  - Bootstrap 5 (full distribution)
  - Custom site.css
  - Component-specific styles (_Layout.cshtml.css)
  - Bootstrap variants (RTL, Grid, Utilities, Reboot)
  - Bootstrap Icons (CDN)

- **JavaScript Libraries**
  - jQuery
  - Bootstrap JavaScript bundle
  - jQuery Validation
  - Custom site.js
  - Toggle scripts for scheduling

### Asset Organization
- Organized library structure in `wwwroot/lib`
- Version-specific asset references
- Minified and development versions available
- CDN integration for Bootstrap Icons

---

## ⚙️ Backend Features

### Middleware Pipeline
Configured in `Program.cs` in the following order:

1. **Exception Handler** (Production only)
   - Route: `/Home/Error`

2. **HSTS** (Production only)
   - HTTP Strict Transport Security

3. **HTTPS Redirection**
   - Forces HTTPS connections

4. **Routing**
   - Enables endpoint routing

5. **Authorization**
   - Ready for authentication/authorization

6. **Static Assets**
   - Serves static files with optimization

7. **MVC Controllers with Views**
   - Enables MVC pattern

### Dependency Injection
- **Service Registration**: `AddControllersWithViews()`
- **Broadcast Service**: `JsonBroadcastService` registered as scoped ⭐ NEW
- Follows ASP.NET Core DI container pattern
- Clean separation of concerns

### Broadcast Service Layer ⭐ NEW
- **Interface**: `IBroadcastService` - Defines contract
- **Implementation**: `JsonBroadcastService` - JSON-based storage
- **Thread-Safe**: SemaphoreSlim for file access protection
- **Async Operations**: All methods use async/await
- **Logging**: Integrated logging for tracking operations

### Configuration
- **appsettings.json**: Production configuration
- **appsettings.Development.json**: Development-specific settings
- Environment-based configuration loading
- No connection strings required (JSON storage)

---

## 📁 Project Structure

```
Broadcast/
├── Controllers/
│   ├── HomeController.cs          # Main application controller
│   └── BroadcastController.cs     # Broadcast CRUD controller ⭐ NEW
├── Models/
│   ├── ErrorViewModel.cs          # Error handling model
│   ├── BroadcastMessage.cs        # Message entity ⭐ NEW
│   └── BroadcastMessageViewModel.cs # Form view model ⭐ NEW
├── Services/                       # Service layer ⭐ NEW
│   ├── IBroadcastService.cs       # Service interface
│   └── JsonBroadcastService.cs    # JSON implementation
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml           # Home page
│   │   └── Privacy.cshtml         # Privacy page
│   ├── Broadcast/                  # Broadcast views ⭐ NEW
│   │   ├── Index.cshtml           # All messages
│   │   ├── Active.cshtml          # Active messages
│   │   ├── Create.cshtml          # Create form
│   │   ├── Edit.cshtml            # Edit form
│   │   └── Details.cshtml         # Message details
│   └── Shared/
│       ├── _Layout.cshtml         # Master layout
│       ├── _Layout.cshtml.css     # Layout styles
│       ├── Error.cshtml           # Error page
│       ├── _ValidationScriptsPartial.cshtml
│       ├── _ViewImports.cshtml    # Global imports
│       └── _ViewStart.cshtml      # View defaults
├── Data/                           # Data storage ⭐ NEW
│   └── broadcast-messages.json    # JSON database
├── wwwroot/
│   ├── css/
│   │   └── site.css               # Custom styles
│   ├── js/
│   │   └── site.js                # Custom scripts
│   └── lib/
│       ├── bootstrap/             # Bootstrap framework
│       ├── jquery/                # jQuery library
│       └── jquery-validation/     # Validation library
├── appsettings.json               # App configuration
├── appsettings.Development.json   # Dev configuration
├── Program.cs                     # Application entry point
├── Broadcast.csproj               # Project file
├── FEATURES.md                    # This file
├── BROADCAST-README.md            # Broadcast guide ⭐ NEW
└── BROADCAST-SUMMARY.md           # Implementation summary ⭐ NEW
```

---

## 🚀 Key Capabilities

### 1. Development Features
- **Hot Reload**: Supports .NET hot reload for rapid development
- **Environment Detection**: Automatic development vs. production configuration
- **Detailed Logging**: Built-in logging infrastructure (configurable)

### 2. Performance Features
- **Static Asset Optimization**: Built-in asset pipeline with versioning
- **Response Caching**: Configurable caching for improved performance
- **Minified Assets**: Production-ready minified CSS and JavaScript

### 3. Extensibility
- **Modular Architecture**: Easy to add new controllers, views, and services
- **Dependency Injection**: Built-in DI container for loose coupling
- **Middleware Pipeline**: Extensible request processing pipeline
- **Tag Helpers**: Razor tag helpers for cleaner view syntax

### 4. Standards & Best Practices
- **Nullable Reference Types**: Enabled for better null safety
- **Implicit Usings**: Cleaner code with reduced using statements
- **Responsive Design**: Mobile-first approach
- **Accessibility**: Semantic HTML and ARIA labels
- **SEO Ready**: Proper meta tags and semantic structure

---

## 📝 Configuration & Settings

### Application Settings
Located in `appsettings.json` and `appsettings.Development.json`:
- Logging configuration
- Connection strings (if needed)
- Environment-specific settings

### Build Configuration
- **Target Framework**: net10.0
- **Nullable**: Enabled
- **Implicit Usings**: Enabled
- **SDK**: Microsoft.NET.Sdk.Web
- **No External Packages**: Uses only built-in .NET libraries

---

## 🔄 Future Enhancement Areas

### **Broadcast System Enhancements**
- User authentication and authorization (ASP.NET Core Identity)
- Role-based permissions (Admin, Editor, Viewer)
- Push notifications (SignalR for real-time updates)
- Email notifications when messages are sent
- SMS integration for urgent messages
- Search and advanced filtering
- Pagination for large message lists
- Message templates and reusable content
- Rich text editor integration
- File attachments support
- Recurring scheduled messages
- Message history and audit trail
- Export functionality (CSV, PDF)
- Analytics dashboard with charts
- A/B testing for messages
- Multi-language support
- Migrate to SQL Server for scale (Entity Framework Core)

### **General Application Enhancements**
- API endpoints (Web API controllers for mobile apps)
- Real-time features (SignalR for live updates)
- Advanced logging (Serilog, Application Insights)
- Caching strategies (Memory cache, Redis)
- Background services (for scheduled message delivery)
- API documentation (Swagger/OpenAPI)
- Unit tests and integration tests
- Docker containerization
- CI/CD pipeline setup

---

## 📞 Project Information
- **Project Name**: Broadcast
- **Framework**: ASP.NET Core 10.0
- **Pattern**: MVC (Model-View-Controller)
- **Language**: C#
- **UI Framework**: Bootstrap 5
- **Data Storage**: JSON Files
- **Key Feature**: Broadcast Message System ⭐

---

## 📚 Additional Documentation

For detailed information about the Broadcast Message System:
- **BROADCAST-README.md** - Complete implementation guide
- **BROADCAST-SUMMARY.md** - Quick reference and feature summary

---

*Last Updated: 2024*
*Version: 1.0 with Broadcast Feature*
