namespace FacebookTimerPosts.DTOs
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime TargetDate { get; set; }
        public int IntervalMinutes { get; set; }
        public string BackgroundImageUrl { get; set; }
        public string FontFamily { get; set; }
        public string PrimaryColor { get; set; }
        public bool HasOverlay { get; set; }
        public string CountdownUrl { get; set; }
        public string Status { get; set; }
        public FacebookPageDto FacebookPage { get; set; }
        public TemplateDto Template { get; set; }
    }
}
