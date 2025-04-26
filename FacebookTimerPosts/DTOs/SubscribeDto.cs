namespace FacebookTimerPosts.DTOs
{
    public class SubscribeDto
    {
        public int SubscriptionPlanId { get; set; }
        public bool AutoRenew { get; set; }
        public string PaymentMethodId { get; set; }
    }
}
