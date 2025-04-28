using FacebookTimerPosts.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacebookTimerPosts.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int FacebookPageId { get; set; }

        [ForeignKey("FacebookPageId")]
        public virtual FacebookPage FacebookPage { get; set; }

        [Required]
        public int TemplateId { get; set; }

        [ForeignKey("TemplateId")]
        public virtual Template Template { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime EventDateTime { get; set; }

        [MaxLength(100)]
        public string CustomFontFamily { get; set; }

        [MaxLength(7)]
        public string CustomPrimaryColor { get; set; }

        public bool ShowDays { get; set; } = true;

        public bool ShowHours { get; set; } = true;

        public bool ShowMinutes { get; set; } = true;

        public bool ShowSeconds { get; set; } = true;

        public string? BackgroundImageUrl { get; set; }

        public bool HasOverlay { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(100)]
        public string FacebookPostId { get; set; }

        [Required]
        public PostStatus Status { get; set; } = PostStatus.Draft;

        public DateTime? PublishedAt { get; set; }

        public DateTime? ScheduledFor { get; set; }

        // New property for refresh functionality
        public int? RefreshIntervalInMinutes { get; set; }

        // New property to track when the post should be refreshed next
        public DateTime? NextRefreshTime { get; set; }
    }
}