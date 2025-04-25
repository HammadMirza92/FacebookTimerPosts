using FacebookTimerPosts.Enums;
using System.ComponentModel.DataAnnotations;

namespace FacebookTimerPosts.DTOs
{
    public class SubscriptionUpdateDto
    {
        [Required]
        public SubscriptionType SubscriptionType { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
