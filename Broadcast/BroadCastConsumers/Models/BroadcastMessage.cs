namespace BroadCastConsumers.Models
{
    public class BroadcastMessage
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public MessagePriority Priority { get; set; }
        public MessageStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ScheduledFor { get; set; }
        public DateTime? SentAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? Category { get; set; }
        public int ViewCount { get; set; }
        public bool IsActive { get; set; }
    }

    public enum MessagePriority
    {
        Low = 1,
        Normal = 2,
        High = 3,
        Urgent = 4
    }

    public enum MessageStatus
    {
        Draft = 1,
        Scheduled = 2,
        Sent = 3,
        Archived = 4
    }
}