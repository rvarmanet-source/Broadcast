using Broadcast.Models;
using System.Text.Json;

namespace Broadcast.Services
{
    public class JsonBroadcastService : IBroadcastService
    {
        private readonly string _jsonFilePath;
        private readonly ILogger<JsonBroadcastService> _logger;
        private readonly IBroadcastMessageSender _messageSender;
        private readonly SemaphoreSlim _fileLock = new(1, 1);

        public JsonBroadcastService(
            IWebHostEnvironment environment, 
            ILogger<JsonBroadcastService> logger,
            IBroadcastMessageSender messageSender)
        {
            _logger = logger;
            _messageSender = messageSender;
            var dataFolder = Path.Combine(environment.ContentRootPath, "Data");
            Directory.CreateDirectory(dataFolder);
            _jsonFilePath = Path.Combine(dataFolder, "broadcast-messages.json");
            InitializeDataFile();
        }

        private void InitializeDataFile()
        {
            if (!File.Exists(_jsonFilePath))
            {
                var initialData = new List<BroadcastMessage>
                {
                    new BroadcastMessage
                    {
                        Id = 1,
                        Title = "Welcome to Broadcast System",
                        Content = "This is your first broadcast message. You can create, schedule, and manage messages from here.",
                        Priority = MessagePriority.Normal,
                        Status = MessageStatus.Sent,
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        SentAt = DateTime.UtcNow.AddDays(-1),
                        CreatedBy = "System",
                        Category = "Announcement",
                        IsActive = true,
                        ViewCount = 0
                    }
                };
                SaveDataAsync(initialData).Wait();
            }
        }

        private async Task<List<BroadcastMessage>> LoadDataAsync()
        {
            await _fileLock.WaitAsync();
            try
            {
                if (!File.Exists(_jsonFilePath))
                    return new List<BroadcastMessage>();

                var json = await File.ReadAllTextAsync(_jsonFilePath);
                return JsonSerializer.Deserialize<List<BroadcastMessage>>(json) ?? new List<BroadcastMessage>();
            }
            finally
            {
                _fileLock.Release();
            }
        }

        private async Task SaveDataAsync(List<BroadcastMessage> messages)
        {
            await _fileLock.WaitAsync();
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                var json = JsonSerializer.Serialize(messages, options);
                await File.WriteAllTextAsync(_jsonFilePath, json);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public async Task<IEnumerable<BroadcastMessage>> GetAllMessagesAsync()
        {
            var messages = await LoadDataAsync();
            return messages.OrderByDescending(m => m.CreatedAt);
        }

        public async Task<IEnumerable<BroadcastMessage>> GetActiveMessagesAsync()
        {
            var messages = await LoadDataAsync();
            return messages
                .Where(m => m.IsActive && m.Status == MessageStatus.Sent)
                .OrderByDescending(m => m.Priority)
                .ThenByDescending(m => m.CreatedAt);
        }

        public async Task<BroadcastMessage?> GetMessageByIdAsync(int id)
        {
            var messages = await LoadDataAsync();
            return messages.FirstOrDefault(m => m.Id == id);
        }

        public async Task<BroadcastMessage> CreateMessageAsync(BroadcastMessageViewModel model)
        {
            var messages = await LoadDataAsync();

            var newId = messages.Any() ? messages.Max(m => m.Id) + 1 : 1;

            var message = new BroadcastMessage
            {
                Id = newId,
                Title = model.Title,
                Content = model.Content,
                Priority = model.Priority,
                Category = model.Category,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Admin",
                IsActive = true,
                ViewCount = 0
            };

            if (model.SendImmediately)
            {
                message.Status = MessageStatus.Sent;
                message.SentAt = DateTime.UtcNow;
            }
            else if (model.ScheduledFor.HasValue)
            {
                message.Status = MessageStatus.Scheduled;
                message.ScheduledFor = model.ScheduledFor;
            }
            else
            {
                message.Status = MessageStatus.Draft;
            }

            messages.Add(message);
            await SaveDataAsync(messages);

            _logger.LogInformation("Broadcast message created: {MessageId} - {Title}", message.Id, message.Title);

            // Send to RabbitMQ if status is Sent
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
        }

        public async Task<bool> UpdateMessageAsync(int id, BroadcastMessageViewModel model)
        {
            var messages = await LoadDataAsync();
            var message = messages.FirstOrDefault(m => m.Id == id);

            if (message == null)
                return false;

            message.Title = model.Title;
            message.Content = model.Content;
            message.Priority = model.Priority;
            message.Category = model.Category;

            if (model.SendImmediately && message.Status != MessageStatus.Sent)
            {
                message.Status = MessageStatus.Sent;
                message.SentAt = DateTime.UtcNow;
            }
            else if (model.ScheduledFor.HasValue)
            {
                message.Status = MessageStatus.Scheduled;
                message.ScheduledFor = model.ScheduledFor;
            }

            await SaveDataAsync(messages);

            _logger.LogInformation("Broadcast message updated: {MessageId}", id);

            // Send to RabbitMQ if status changed to Sent
            if (model.SendImmediately && message.Status == MessageStatus.Sent)
            {
                try
                {
                    await _messageSender.SendMessageAsync(message);
                    _logger.LogInformation("Updated message sent to RabbitMQ: {MessageId}", message.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send updated message to RabbitMQ: {MessageId}", message.Id);
                }
            }

            return true;
        }

        public async Task<bool> DeleteMessageAsync(int id)
        {
            var messages = await LoadDataAsync();
            var message = messages.FirstOrDefault(m => m.Id == id);

            if (message == null)
                return false;

            message.IsActive = false;
            message.Status = MessageStatus.Archived;
            await SaveDataAsync(messages);

            _logger.LogInformation("Broadcast message archived: {MessageId}", id);

            return true;
        }

        public async Task<bool> SendMessageAsync(int id)
        {
            var messages = await LoadDataAsync();
            var message = messages.FirstOrDefault(m => m.Id == id);

            if (message == null)
                return false;

            message.Status = MessageStatus.Sent;
            message.SentAt = DateTime.UtcNow;
            await SaveDataAsync(messages);

            _logger.LogInformation("Broadcast message sent: {MessageId}", id);

            // Send to RabbitMQ
            try
            {
                await _messageSender.SendMessageAsync(message);
                _logger.LogInformation("Message published to RabbitMQ: {MessageId}", message.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message to RabbitMQ: {MessageId}", message.Id);
                // Note: Message is still marked as sent in local storage
            }

            return true;
        }

        public async Task<int> GetActiveMessageCountAsync()
        {
            var messages = await LoadDataAsync();
            return messages.Count(m => m.IsActive && m.Status == MessageStatus.Sent);
        }

        public async Task IncrementViewCountAsync(int id)
        {
            var messages = await LoadDataAsync();
            var message = messages.FirstOrDefault(m => m.Id == id);

            if (message != null)
            {
                message.ViewCount++;
                await SaveDataAsync(messages);
            }
        }
    }
}
