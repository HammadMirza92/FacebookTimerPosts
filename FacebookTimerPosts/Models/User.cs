using FacebookTimerPosts.Enums;
using Microsoft.Extensions.Hosting;

namespace FacebookTimerPosts.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public SubscriptionType SubscriptionType { get; set; } = SubscriptionType.Free;
        public DateTime? SubscriptionEndDate { get; set; }
        public ICollection<FacebookPage> LinkedPages { get; set; } = new List<FacebookPage>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
