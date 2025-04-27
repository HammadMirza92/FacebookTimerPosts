using FacebookTimerPosts.Enums;

namespace FacebookTimerPosts.DTOs
{
    public class PostDto
    {
        public int Id { get; set; }
        public int FacebookPageId { get; set; }
        public string FacebookPageName { get; set; }
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
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
        public int? RefreshIntervalInMinutes { get; set; }
        public string CountdownPublicId { get; set; }
    }
}
