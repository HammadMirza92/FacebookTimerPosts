using System.ComponentModel.DataAnnotations;

namespace FacebookTimerPosts.DTOs
{
    public class UpdatePostDto
    {
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

        public string BackgroundImageUrl { get; set; }

        public bool HasOverlay { get; set; }

        public DateTime? ScheduledFor { get; set; }

        public int? RefreshIntervalInMinutes { get; set; }

        // This will be calculated automatically
        public DateTime? NextRefreshTime { get; set; }
    }
}
