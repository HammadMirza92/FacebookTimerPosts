namespace FacebookTimerPosts.DTOs
{
    public class UserSubscriptionDto
    {
        public int Id { get; set; }
        public int SubscriptionPlanId { get; set; }
        public string SubscriptionPlanName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool AutoRenew { get; set; }
        public int DaysRemaining { get; set; }
    }
}
