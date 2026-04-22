# 🔧 RabbitMQ Integration Fix - Troubleshooting Guide

## 🐛 Issue: BroadcastMessageSender Not Being Called

### Problem Identified
The `BroadcastMessageSender` was not being called after creating messages because the RabbitMQ sending code was missing from the `JsonBroadcastService`.

### ✅ Fix Applied

Updated **3 methods** in `JsonBroadcastService.cs`:

1. **CreateMessageAsync** - Now sends to RabbitMQ when `SendImmediately` is checked
2. **UpdateMessageAsync** - Now sends to RabbitMQ when status changes to Sent
3. **SendMessageAsync** - Already had the code (working correctly)

---

## 🧪 How to Test the Fix

### 1. Verify RabbitMQ is Running

**Check Docker:**
```powershell
docker ps
```

**Should see:**
```
CONTAINER ID   IMAGE                  STATUS          PORTS
abc123def456   rabbitmq:3-management  Up 2 hours      0.0.0.0:5672->5672/tcp, 0.0.0.0:15672->15672/tcp
```

**If not running:**
```powershell
docker start rabbitmq
```

**Or start fresh:**
```powershell
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

---

### 2. Enable Debug Logging (Optional but Recommended)

**Update `appsettings.Development.json`:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Warning",
      "Broadcast.Services": "Debug"
    }
  }
}
```

---

### 3. Run Your Application

```powershell
dotnet run
```

---

### 4. Test Message Creation

#### Option A: Web UI
1. Navigate to: http://localhost:5001/Broadcast
2. Click **"Create New Message"**
3. Fill in the form:
   - **Title**: "Test RabbitMQ Integration"
   - **Content**: "Testing message queue"
   - **Priority**: High
   - **Category**: Test
   - **✅ Check "Send Immediately"** ← Important!
4. Click **"Create Message"**

#### Option B: Test Without "Send Immediately"
1. Create message without checking "Send Immediately"
2. Message status = Draft (NOT sent to RabbitMQ)
3. Click **"Send Now"** button (paper plane icon)
4. Now it should send to RabbitMQ

---

### 5. Check Application Logs

**Look for these log messages:**

#### ✅ Success Logs:
```
[Information] Broadcast message created: 1 - Test RabbitMQ Integration
[Information] Message sent to RabbitMQ: 1
[Information] Message published to RabbitMQ - ID: 1, Title: Test RabbitMQ Integration, Priority: High
```

#### ❌ Error Logs (if RabbitMQ not running):
```
[Error] Failed to send message to RabbitMQ: 1
System.InvalidOperationException: RabbitMQ channel is not initialized
```

---

### 6. Verify in RabbitMQ Management UI

1. **Open RabbitMQ UI**: http://localhost:15672
2. **Login**: guest / guest
3. **Go to**: Queues tab
4. **Find**: `broadcast-messages` queue
5. **Check**: "Ready" column should show 1 message

#### View Message Details:
1. Click on **`broadcast-messages`** queue name
2. Scroll to **"Get messages"** section
3. Set **Messages**: 1
4. Click **"Get Message(s)"**
5. Click **"Payload"** to see JSON

**Expected Payload:**
```json
{
  "Id": 1,
  "Title": "Test RabbitMQ Integration",
  "Content": "Testing message queue",
  "Priority": 3,
  "Status": 3,
  "CreatedAt": "2024-01-15T10:30:00Z",
  "SentAt": "2024-01-15T10:30:00Z",
  "CreatedBy": "Admin",
  "Category": "Test",
  "ViewCount": 0,
  "IsActive": true
}
```

**Expected Headers:**
```
priority: 3
status: Sent
category: Test
```

---

## 🔍 Diagnostic Checklist

### ✅ Pre-Flight Checklist

- [ ] RabbitMQ is running (Docker or service)
- [ ] Application builds successfully (`dotnet build`)
- [ ] No errors in Visual Studio Error List
- [ ] appsettings.json has RabbitMQ section

### ✅ Testing Checklist

- [ ] Application starts without errors
- [ ] Create message page loads
- [ ] "Send Immediately" checkbox works
- [ ] Message creates successfully
- [ ] Success notification appears
- [ ] Logs show RabbitMQ publish message
- [ ] RabbitMQ UI shows message in queue

---

## 🐛 Common Issues & Solutions

### Issue 1: "Connection refused" in logs

**Cause**: RabbitMQ is not running

**Solution**:
```powershell
# Check if RabbitMQ container exists
docker ps -a | findstr rabbitmq

# Start existing container
docker start rabbitmq

# Or create new one
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

---

### Issue 2: "Authentication failed"

**Cause**: Wrong credentials in appsettings.json

**Solution**:
Check `appsettings.json`:
```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",   ← Must be "guest"
    "Password": "guest"    ← Must be "guest"
  }
}
```

---

### Issue 3: No logs showing RabbitMQ activity

**Cause**: Logging level too high

**Solution**:
Update `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Broadcast.Services": "Information"
    }
  }
}
```

---

### Issue 4: Message creates but not in RabbitMQ

**Cause 1**: "Send Immediately" not checked
- **Solution**: Check the box when creating

**Cause 2**: Message status is Draft or Scheduled
- **Solution**: Click "Send Now" button after creation

**Cause 3**: RabbitMQ sender initialization failed
- **Solution**: Check application startup logs for errors

---

### Issue 5: Exception during message send

**Check logs for specific error:**

**Error**: `Channel is not initialized`
```
[Error] RabbitMQ channel is not initialized
```
**Solution**: 
1. Check RabbitMQ connection settings
2. Verify RabbitMQ is accessible
3. Restart application

---

## 📊 Verification Steps Summary

### Step 1: Check Application Logs
```powershell
# In Visual Studio Output window, select "Debug" or "Web Server"
# Look for these patterns:

[Information] Broadcast message created: {ID} - {Title}
[Information] Message sent to RabbitMQ: {ID}
[Information] Message published to RabbitMQ - ID: {ID}
```

### Step 2: Check RabbitMQ Logs
```powershell
# If using Docker:
docker logs rabbitmq --tail 50

# Look for:
# - Connection accepted
# - Channel created
# - Message published
```

### Step 3: Check Queue in RabbitMQ UI
1. http://localhost:15672
2. Queues → broadcast-messages
3. Should show: Ready: 1, Total: 1

### Step 4: Get Message from Queue (Testing)
1. In RabbitMQ UI, click queue name
2. "Get messages" section
3. Click "Get Message(s)"
4. Verify payload and headers

---

## 🎯 Expected Behavior After Fix

### Creating Message with "Send Immediately" ✅

**Flow:**
```
User creates message
       ↓
BroadcastController.Create()
       ↓
JsonBroadcastService.CreateMessageAsync()
       ↓
Check: model.SendImmediately == true?
       ↓ YES
Set: message.Status = MessageStatus.Sent
       ↓
Save to JSON file
       ↓
Call: _messageSender.SendMessageAsync(message) ⭐ FIX APPLIED
       ↓
BroadcastMessageSender publishes to RabbitMQ
       ↓
Message appears in queue
```

**Logs:**
```
[13:45:12 INF] Broadcast message created: 1 - Test Message
[13:45:12 INF] Message sent to RabbitMQ: 1
[13:45:12 INF] RabbitMQ connection initialized successfully
[13:45:12 INF] Message published to RabbitMQ - ID: 1, Title: Test Message, Priority: High
```

---

### Editing Message with "Send Immediately" ✅

**Flow:**
```
User edits message
       ↓
Checks "Send Immediately"
       ↓
BroadcastController.Edit()
       ↓
JsonBroadcastService.UpdateMessageAsync()
       ↓
Check: model.SendImmediately && status changed?
       ↓ YES
Call: _messageSender.SendMessageAsync(message) ⭐ FIX APPLIED
       ↓
Published to RabbitMQ
```

---

### Manually Sending Draft Message ✅

**Flow:**
```
User clicks "Send Now" button
       ↓
BroadcastController.Send()
       ↓
JsonBroadcastService.SendMessageAsync(id)
       ↓
Set: message.Status = MessageStatus.Sent
       ↓
Call: _messageSender.SendMessageAsync(message) ⭐ ALREADY WORKING
       ↓
Published to RabbitMQ
```

---

## 📝 Quick Test Script

**PowerShell script to verify everything:**

```powershell
# 1. Check RabbitMQ
Write-Host "Checking RabbitMQ..." -ForegroundColor Cyan
docker ps | findstr rabbitmq
if ($LASTEXITCODE -ne 0) {
    Write-Host "RabbitMQ not running! Starting..." -ForegroundColor Yellow
    docker start rabbitmq
}

# 2. Build application
Write-Host "`nBuilding application..." -ForegroundColor Cyan
dotnet build
if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful!" -ForegroundColor Green
} else {
    Write-Host "Build failed!" -ForegroundColor Red
    exit
}

# 3. Instructions
Write-Host "`n=== READY TO TEST ===" -ForegroundColor Green
Write-Host "1. Run: dotnet run"
Write-Host "2. Open: http://localhost:5001/Broadcast"
Write-Host "3. Create message with 'Send Immediately' checked"
Write-Host "4. Check RabbitMQ UI: http://localhost:15672"
Write-Host "5. Look for message in 'broadcast-messages' queue"
```

**Save as**: `test-rabbitmq.ps1` and run:
```powershell
.\test-rabbitmq.ps1
```

---

## 🎓 Understanding the Fix

### What Was Wrong?

The original `JsonBroadcastService` had:
```csharp
// CreateMessageAsync - BEFORE
messages.Add(message);
await SaveDataAsync(messages);
_logger.LogInformation("Broadcast message created: {MessageId}", message.Id);
return message; // ❌ NO RabbitMQ call!
```

### What Was Fixed?

Now it has:
```csharp
// CreateMessageAsync - AFTER
messages.Add(message);
await SaveDataAsync(messages);
_logger.LogInformation("Broadcast message created: {MessageId}", message.Id);

// Send to RabbitMQ if status is Sent ✅ ADDED
if (message.Status == MessageStatus.Sent)
{
    try
    {
        await _messageSender.SendMessageAsync(message);
        _logger.LogInformation("Message sent to RabbitMQ: {MessageId}", message.Id);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to send message to RabbitMQ: {MessageId}", message.Id);
    }
}

return message;
```

### Why It Works Now?

1. **Condition Check**: Only sends when `message.Status == MessageStatus.Sent`
2. **Async Call**: Properly awaits the RabbitMQ sender
3. **Error Handling**: Catches RabbitMQ failures without breaking message creation
4. **Logging**: Logs both success and failure for debugging

---

## ✅ Success Criteria

You know it's working when:

1. ✅ Application starts without errors
2. ✅ Creating message with "Send Immediately" shows success
3. ✅ Logs show: "Message sent to RabbitMQ: {ID}"
4. ✅ Logs show: "Message published to RabbitMQ - ID: {ID}"
5. ✅ RabbitMQ UI shows message count increased
6. ✅ Message payload visible in RabbitMQ UI
7. ✅ No exceptions in application logs

---

## 📞 Need More Help?

### Check These Files:
- `JsonBroadcastService.cs` - Service with RabbitMQ calls
- `BroadcastMessageSender.cs` - RabbitMQ publisher
- `Program.cs` - Service registration
- `appsettings.json` - RabbitMQ configuration

### Review Documentation:
- `RABBITMQ-INTEGRATION.md` - Complete guide
- `RABBITMQ-QUICKSTART.md` - Quick reference
- `RABBITMQ-SUMMARY.md` - Implementation summary

---

**Your fix is ready! Test it now! 🚀**

*Last Updated: 2024*
*Status: Fix Applied ✅*
