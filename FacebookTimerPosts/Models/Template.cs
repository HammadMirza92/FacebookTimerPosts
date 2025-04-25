using FacebookTimerPosts.Enums;

namespace FacebookTimerPosts.Models
{
    public class Template
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PreviewImageUrl { get; set; }
        public string BackgroundImageUrl { get; set; }
        public string FontFamily { get; set; } = "Anton";
        public string PrimaryColor { get; set; } = "#FFFFFF";
        public bool HasOverlay { get; set; } = true;
        public TemplateCategory Category { get; set; }
        public SubscriptionType MinimumSubscription { get; set; } = SubscriptionType.Free;
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
