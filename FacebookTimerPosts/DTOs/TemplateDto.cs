namespace FacebookTimerPosts.DTOs
{
    public class TemplateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PreviewImageUrl { get; set; }
        public string BackgroundImageUrl { get; set; }
        public string FontFamily { get; set; }
        public string PrimaryColor { get; set; }
        public bool HasOverlay { get; set; }
        public string Category { get; set; }
        public string MinimumSubscription { get; set; }
    }
}
