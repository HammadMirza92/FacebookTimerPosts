namespace FacebookTimerPosts.DTOs
{
    public class PostPreviewDto
    {
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
    }
}
