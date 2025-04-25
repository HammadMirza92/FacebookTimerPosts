using FacebookTimerPosts.Enums;

namespace FacebookTimerPosts.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public DateTime TargetDate { get; set; }
        public int IntervalMinutes { get; set; } = 15;
        public string BackgroundImageUrl { get; set; }
        public string FontFamily { get; set; } = "Anton";
        public string PrimaryColor { get; set; } = "#FFFFFF";
        public bool HasOverlay { get; set; } = true;
        public string CountdownUrl { get; set; }
        public PostStatus Status { get; set; } = PostStatus.Draft;
        public int UserId { get; set; }
        public User User { get; set; }
        public int? FacebookPageId { get; set; }
        public FacebookPage FacebookPage { get; set; }
        public int? TemplateId { get; set; }
        public Template Template { get; set; }
    }
}
