using Broadcast.Models;

namespace Broadcast.Services
{
    public class BroadcastService : IBroadcastService
    {
        private static readonly List<BroadcastMessage> _messages = new();
        private static int _nextId = 1;
        private readonly ILogger<BroadcastService> _logger;

        public BroadcastService(ILogger<BroadcastService> logger)
        {
            _logger = logger;
        }

        public Task<IEnumerable<BroadcastMessage>> GetAllMessagesAsync()
        {
            var messages = _messages
                .OrderByDescending(m => m.CreatedAt)
                .AsEnumerable();
            return Task.FromResult(messages);
        }

        public Task<IEnumerable<BroadcastMessage>> GetActiveMessagesAsync()
        {
            var messages = _messages
                .Where(m => m.IsActive && m.Status == MessageStatus.Sent)
                .OrderByDescending(m => m.Priority)
                .ThenByDescending(m => m.CreatedAt)
                .AsEnumerable();
            return Task.FromResult(messages);
        }

        public Task<BroadcastMessage?> GetMessageByIdAsync(int id)
        {
            var message = _messages.FirstOrDefault(m => m.Id == id);
            return Task.FromResult(message);
        }

        public Task<BroadcastMessage> CreateMessageAsync(BroadcastMessageViewModel model)
        {
            var message = new BroadcastMessage
            {
                Id = _nextId++,
                Title = model.Title,
                Content = model.Content,
                Priority = model.Priority,
                Category = model.Category,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Admin", // TODO: Get from authentication
                IsActive = true
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

            _messages.Add(message);

            _logger.LogInformation("Broadcast message created: {MessageId} - {Title}", message.Id, message.Title);

            return Task.FromResult(message);
        }

        public Task<bool> UpdateMessageAsync(int id, BroadcastMessageViewModel model)
        {
            var message = _messages.FirstOrDefault(m => m.Id == id);
            if (message == null)
                return Task.FromResult(false);

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

            _logger.LogInformation("Broadcast message updated: {MessageId}", id);

            return Task.FromResult(true);
        }

        public Task<bool> DeleteMessageAsync(int id)
        {
            var message = _messages.FirstOrDefault(m => m.Id == id);
            if (message == null)
                return Task.FromResult(false);

            message.IsActive = false;
            message.Status = MessageStatus.Archived;

            _logger.LogInformation("Broadcast message archived: {MessageId}", id);

            return Task.FromResult(true);
        }

        public Task<bool> SendMessageAsync(int id)
        {
            var message = _messages.FirstOrDefault(m => m.Id == id);
            if (message == null)
                return Task.FromResult(false);

            message.Status = MessageStatus.Sent;
            message.SentAt = DateTime.UtcNow;

            _logger.LogInformation("Broadcast message sent: {MessageId}", id);

            return Task.FromResult(true);
        }

        public Task<int> GetActiveMessageCountAsync()
        {
            var count = _messages.Count(m => m.IsActive && m.Status == MessageStatus.Sent);
            return Task.FromResult(count);
        }

        public Task IncrementViewCountAsync(int id)
        {
            var message = _messages.FirstOrDefault(m => m.Id == id);
            if (message != null)
            {
                message.ViewCount++;
            }
            return Task.CompletedTask;
        }
    }
}