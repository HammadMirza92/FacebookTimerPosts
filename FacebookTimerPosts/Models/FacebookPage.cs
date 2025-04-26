using Microsoft.Extensions.Hosting;

namespace FacebookTimerPosts.Models
{
    public class FacebookPage
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string PageId { get; set; }
        public string PageName { get; set; }
        public string PageAccessToken { get; set; }
        public DateTime TokenExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Post> Posts { get; set; }

        public FacebookPage()
        {
            Posts = new HashSet<Post>();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }
    }
}
