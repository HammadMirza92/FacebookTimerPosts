using FacebookTimerPosts.Enums;

namespace FacebookTimerPosts.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int FacebookPageId { get; set; }
        public int TemplateId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventDateTime { get; set; }
        public string CustomFontFamily { get; set; }
        public string CustomPrimaryColor { get; set; }
        public bool ShowDays { get; set; }
        public bool ShowHours { get; set; }
        public bool ShowMinutes { get; set; }
        public bool ShowSeconds { get; set; }
        public string BackgroundImageUrl { get; set; }
        public bool HasOverlay { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string FacebookPostId { get; set; }
        public PostStatus Status { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? ScheduledFor { get; set; }
        public int RefreshIntervalInMinutes { get; set; }

        public virtual User User { get; set; }
        public virtual FacebookPage FacebookPage { get; set; }
        public virtual Template Template { get; set; }

        public Post()
        {
            CreatedAt = DateTime.UtcNow;
            Status = PostStatus.Draft;
            ShowDays = true;
            ShowHours = true;
            ShowMinutes = true;
            RefreshIntervalInMinutes = 15;
        }
    }
}
