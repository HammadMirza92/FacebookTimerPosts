namespace FacebookTimerPosts.Models
{
    public class UserSubscription
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int SubscriptionPlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool AutoRenew { get; set; }
        public string PaymentReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User User { get; set; }
        public virtual SubscriptionPlan SubscriptionPlan { get; set; }

        public UserSubscription()
        {
            StartDate = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }
    }
}
