# Broadcast System - Architecture Overview

## 🏗️ System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         User Interface                           │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌─────────────┐  ┌──────────────┐  ┌──────────────────────┐   │
│  │   Index     │  │   Create     │  │   Active Messages    │   │
│  │  (All Msgs) │  │  (New Msg)   │  │   (Public View)      │   │
│  └─────────────┘  └──────────────┘  └──────────────────────┘   │
│                                                                   │
│  ┌─────────────┐  ┌──────────────┐                              │
│  │   Edit      │  │   Details    │                              │
│  │  (Modify)   │  │   (View)     │                              │
│  └─────────────┘  └──────────────┘                              │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘
                              ↓ ↑
┌─────────────────────────────────────────────────────────────────┐
│                     Broadcast Controller                         │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  GET:  Index(), Active(), Create(), Edit(), Details()            │
│  POST: Create(), Edit(), Send(), Delete()                        │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘
                              ↓ ↑
┌─────────────────────────────────────────────────────────────────┐
│                    IBroadcastService                             │
│                  (Service Interface)                             │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  • GetAllMessagesAsync()                                         │
│  • GetActiveMessagesAsync()                                      │
│  • GetMessageByIdAsync(id)                                       │
│  • CreateMessageAsync(model)                                     │
│  • UpdateMessageAsync(id, model)                                 │
│  • DeleteMessageAsync(id)                                        │
│  • SendMessageAsync(id)                                          │
│  • IncrementViewCountAsync(id)                                   │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘
                              ↓ ↑
┌─────────────────────────────────────────────────────────────────┐
│               JsonBroadcastService                               │
│            (Service Implementation)                              │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  • Thread-safe file operations (SemaphoreSlim)                   │
│  • JSON serialization/deserialization                            │
│  • Automatic ID generation                                       │
│  • Logging integration                                           │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘
                              ↓ ↑
┌─────────────────────────────────────────────────────────────────┐
│                  Data Storage Layer                              │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│              Data/broadcast-messages.json                        │
│                                                                   │
│  [                                                                │
│    {                                                              │
│      "Id": 1,                                                     │
│      "Title": "Message Title",                                    │
│      "Content": "Message content...",                             │
│      "Priority": 2,                                               │
│      "Status": 3,                                                 │
│      "CreatedAt": "2024-01-15T10:30:00Z",                         │
│      "ViewCount": 5,                                              │
│      ...                                                          │
│    }                                                              │
│  ]                                                                │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘
```

---

## 📊 Request Flow Diagram

### Creating a Message

```
User                Controller             Service              JSON File
  │                     │                    │                     │
  │  Fill Create Form   │                    │                     │
  ├────────────────────>│                    │                     │
  │                     │                    │                     │
  │                     │ CreateMessageAsync │                     │
  │                     ├───────────────────>│                     │
  │                     │                    │                     │
  │                     │                    │  Read current data  │
  │                     │                    ├────────────────────>│
  │                     │                    │<────────────────────┤
  │                     │                    │                     │
  │                     │                    │  Generate new ID    │
  │                     │                    │  Create message     │
  │                     │                    │  Add to list        │
  │                     │                    │                     │
  │                     │                    │  Write updated data │
  │                     │                    ├────────────────────>│
  │                     │                    │<────────────────────┤
  │                     │                    │                     │
  │                     │<───────────────────┤                     │
  │                     │  Message created   │                     │
  │                     │                    │                     │
  │  Redirect to Index  │                    │                     │
  │<────────────────────┤                    │                     │
  │                     │                    │                     │
```

### Viewing Active Messages

```
User                Controller             Service              JSON File
  │                     │                    │                     │
  │  Click "Active"     │                    │                     │
  ├────────────────────>│                    │                     │
  │                     │                    │                     │
  │                     │GetActiveMessagesAsync                    │
  │                     ├───────────────────>│                     │
  │                     │                    │                     │
  │                     │                    │  Read JSON file     │
  │                     │                    ├────────────────────>│
  │                     │                    │<────────────────────┤
  │                     │                    │                     │
  │                     │                    │  Filter active      │
  │                     │                    │  Sort by priority   │
  │                     │                    │                     │
  │                     │<───────────────────┤                     │
  │                     │  Active messages   │                     │
  │                     │                    │                     │
  │  Display cards      │                    │                     │
  │<────────────────────┤                    │                     │
  │                     │                    │                     │
```

---

## 🔄 Message Status Lifecycle

```
┌──────────┐
│  DRAFT   │ ← Created without "Send Immediately"
└────┬─────┘
     │
     │ (Manual Send)
     ↓
┌──────────┐
│SCHEDULED │ ← Created with future date
└────┬─────┘
     │
     │ (Manual Send or Scheduled Time)
     ↓
┌──────────┐
│  SENT    │ ← Active and visible to users
└────┬─────┘
     │
     │ (Archive/Delete)
     ↓
┌──────────┐
│ ARCHIVED │ ← Soft deleted (IsActive = false)
└──────────┘
```

---

## 🎯 Priority Levels

```
┌──────────────────────────────────────────┐
│  4. URGENT   │ 🔴 Red    │ Critical     │
├──────────────────────────────────────────┤
│  3. HIGH     │ 🟠 Orange │ Important    │
├──────────────────────────────────────────┤
│  2. NORMAL   │ 🔵 Blue   │ Regular      │
├──────────────────────────────────────────┤
│  1. LOW      │ ⚫ Gray   │ Optional     │
└──────────────────────────────────────────┘
```

---

## 🗂️ Data Model

```
BroadcastMessage
├── Id: int (Auto-generated)
├── Title: string (max 200)
├── Content: string (max 2000)
├── Priority: enum (Low, Normal, High, Urgent)
├── Status: enum (Draft, Scheduled, Sent, Archived)
├── CreatedAt: DateTime
├── ScheduledFor: DateTime? (nullable)
├── SentAt: DateTime? (nullable)
├── CreatedBy: string
├── Category: string? (nullable)
├── ViewCount: int
└── IsActive: bool
```

---

## 🔐 Security Features

```
┌─────────────────────────────────────────┐
│          Current Security               │
├─────────────────────────────────────────┤
│ ✅ Anti-Forgery Tokens (CSRF)           │
│ ✅ Input Validation (Data Annotations)  │
│ ✅ HTTPS Redirection                    │
│ ✅ HSTS (Production)                    │
│ ✅ ModelState Validation                │
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│       Future Enhancements               │
├─────────────────────────────────────────┤
│ 🔜 Authentication (ASP.NET Identity)    │
│ 🔜 Authorization (Role-based)           │
│ 🔜 XSS Protection (Content Security)    │
│ 🔜 Rate Limiting                        │
│ 🔜 Audit Logging                        │
└─────────────────────────────────────────┘
```

---

## 📱 User Roles (Future)

```
┌──────────────────────────────────────────────────────┐
│                    SUPER ADMIN                       │
│  • Full access to all features                       │
│  • Manage users and permissions                      │
└──────────────────────────────────────────────────────┘
                      │
        ┌─────────────┴─────────────┐
        │                           │
┌───────▼──────┐            ┌───────▼──────┐
│    ADMIN     │            │   EDITOR     │
│  • CRUD all  │            │  • CRUD own  │
│  • Send msgs │            │  • Request   │
│  • Analytics │            │    approval  │
└──────────────┘            └──────────────┘
        │
        │
┌───────▼──────┐
│    VIEWER    │
│  • Read only │
│  • View msgs │
└──────────────┘
```

---

## 🚀 Deployment Architecture

### Current (Development)

```
┌─────────────────────────────────────┐
│         Local Machine               │
│  ┌──────────────────────────────┐   │
│  │   ASP.NET Core App           │   │
│  │   (Kestrel Server)           │   │
│  │                              │   │
│  │   ┌──────────────────────┐   │   │
│  │   │  broadcast-msg.json  │   │   │
│  │   └──────────────────────┘   │   │
│  └──────────────────────────────┘   │
└─────────────────────────────────────┘
```

### Production (Recommended)

```
┌─────────────────────────────────────────────┐
│           Cloud Platform (Azure/AWS)        │
│  ┌──────────────────────────────────────┐   │
│  │         Web Server (IIS/Nginx)       │   │
│  │  ┌────────────────────────────────┐  │   │
│  │  │   ASP.NET Core App             │  │   │
│  │  └────────────────────────────────┘  │   │
│  └──────────────────────────────────────┘   │
│                    │                         │
│                    ↓                         │
│  ┌──────────────────────────────────────┐   │
│  │   Database (SQL Server/PostgreSQL)   │   │
│  │   or                                 │   │
│  │   Blob Storage (for JSON)            │   │
│  └──────────────────────────────────────┘   │
└─────────────────────────────────────────────┘
```

---

## 🔄 CI/CD Pipeline (Future)

```
Developer          GitHub              Build              Deploy
    │                │                   │                  │
    │  git push      │                   │                  │
    ├───────────────>│                   │                  │
    │                │   Trigger build   │                  │
    │                ├──────────────────>│                  │
    │                │                   │                  │
    │                │                   │  Run tests       │
    │                │                   │  Build app       │
    │                │                   │  Create package  │
    │                │                   │                  │
    │                │                   │   Deploy to      │
    │                │                   │   staging        │
    │                │                   ├─────────────────>│
    │                │                   │                  │
    │                │                   │   Deploy to      │
    │                │                   │   production     │
    │                │                   ├─────────────────>│
    │                │                   │                  │
```

---

## 📊 Performance Considerations

### Current Implementation
- **Suitable for**: < 10,000 messages
- **Read performance**: O(n) - linear scan
- **Write performance**: O(1) - append + serialize
- **Concurrency**: Thread-safe with SemaphoreSlim

### Optimization Strategies
1. **Caching**: In-memory cache for frequently accessed messages
2. **Indexing**: Add in-memory indices for common queries
3. **Database**: Migrate to SQL for > 10,000 messages
4. **CDN**: Serve static assets from CDN
5. **Compression**: Enable response compression

---

This architecture provides a solid foundation that can scale from a simple JSON-based system to a full enterprise solution! 🚀
