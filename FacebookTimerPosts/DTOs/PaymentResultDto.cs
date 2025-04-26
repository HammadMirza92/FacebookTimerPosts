using FacebookTimerPosts.Enums;

namespace FacebookTimerPosts.DTOs
{
    public class PaymentResultDto
    {
        public int Id { get; set; }
        public int? UserSubscriptionId { get; set; }
        public string PaymentProvider { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public PaymentStatus Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
