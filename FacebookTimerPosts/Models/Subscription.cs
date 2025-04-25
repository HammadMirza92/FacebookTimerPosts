using FacebookTimerPosts.Enums;

namespace FacebookTimerPosts.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public SubscriptionType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string PaymentReference { get; set; }
    }
}
