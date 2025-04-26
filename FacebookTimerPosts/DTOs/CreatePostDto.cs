namespace FacebookTimerPosts.DTOs
{
    public class CreatePostDto
    {
        public int FacebookPageId { get; set; }
        public int TemplateId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventDateTime { get; set; }
        public string CustomFontFamily { get; set; }
        public string CustomPrimaryColor { get; set; }
        public bool ShowDays { get; set; } = true;
        public bool ShowHours { get; set; } = true;
        public bool ShowMinutes { get; set; } = true;
        public bool ShowSeconds { get; set; } = true;
        public string BackgroundImageUrl { get; set; }
        public bool HasOverlay { get; set; }
        public DateTime? ScheduledFor { get; set; }
        public int RefreshIntervalInMinutes { get; set; } = 15;
    }
}
