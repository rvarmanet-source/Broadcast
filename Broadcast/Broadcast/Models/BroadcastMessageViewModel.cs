using System.ComponentModel.DataAnnotations;

namespace Broadcast.Models
{
    public class BroadcastMessageViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Message Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message content is required")]
        [StringLength(2000, ErrorMessage = "Message cannot exceed 2000 characters")]
        [Display(Name = "Message Content")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Priority is required")]
        [Display(Name = "Priority Level")]
        public MessagePriority Priority { get; set; }

        [Display(Name = "Schedule For Later")]
        public DateTime? ScheduledFor { get; set; }

        [Display(Name = "Category")]
        [StringLength(50)]
        public string? Category { get; set; }

        [Display(Name = "Send Immediately")]
        public bool SendImmediately { get; set; } = true;
    }
}