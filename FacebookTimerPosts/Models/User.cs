using FacebookTimerPosts.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace FacebookTimerPosts.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; }
        public virtual ICollection<FacebookPage> FacebookPages { get; set; }
        public virtual ICollection<Post> Posts { get; set; }

        public User()
        {
            UserSubscriptions = new HashSet<UserSubscription>();
            FacebookPages = new HashSet<FacebookPage>();
            Posts = new HashSet<Post>();
            RegistrationDate = DateTime.UtcNow;
            IsActive = true;
        }
    }
}
