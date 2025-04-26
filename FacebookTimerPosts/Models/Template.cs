using FacebookTimerPosts.Enums;

namespace FacebookTimerPosts.Models
{
    public class Template
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? BackgroundImageUrl { get; set; }
        public string DefaultFontFamily { get; set; }
        public string PrimaryColor { get; set; }
        public bool RequiresSubscription { get; set; }
        public int? MinimumSubscriptionPlanId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual SubscriptionPlan MinimumSubscriptionPlan { get; set; }
        public virtual ICollection<Post> Posts { get; set; }

        public Template()
        {
            Posts = new HashSet<Post>();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }
    }
}
