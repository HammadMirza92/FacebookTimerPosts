using FacebookTimerPosts.Enums;

namespace FacebookTimerPosts.Models
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }
        public int MaxPosts { get; set; }
        public int MaxTemplates { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual ICollection<Template> AvailableTemplates { get; set; }
        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; }

        public SubscriptionPlan()
        {
            AvailableTemplates = new HashSet<Template>();
            UserSubscriptions = new HashSet<UserSubscription>();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }
    }
}
