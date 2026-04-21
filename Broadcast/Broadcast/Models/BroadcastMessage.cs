using System.ComponentModel.DataAnnotations;

namespace Broadcast.Models
{
    public class BroadcastMessage
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message content is required")]
        [StringLength(2000, ErrorMessage = "Message cannot exceed 2000 characters")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Priority is required")]
        public MessagePriority Priority { get; set; }

        public MessageStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ScheduledFor { get; set; }

        public DateTime? SentAt { get; set; }

        public string CreatedBy { get; set; } = "System";

        public string? Category { get; set; }

        public int ViewCount { get; set; }

        public bool IsActive { get; set; } = true;
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