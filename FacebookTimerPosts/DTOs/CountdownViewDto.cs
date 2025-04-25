namespace FacebookTimerPosts.DTOs
{
    public class CountdownViewDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime TargetDate { get; set; }
        public int IntervalMinutes { get; set; }
        public string BackgroundImageUrl { get; set; }
        public string FontFamily { get; set; }
        public string PrimaryColor { get; set; }
        public bool HasOverlay { get; set; }
    }
}
