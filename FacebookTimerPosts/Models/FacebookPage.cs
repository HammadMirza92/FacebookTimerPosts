using Microsoft.Extensions.Hosting;

namespace FacebookTimerPosts.Models
{
    public class FacebookPage
    {
        public int Id { get; set; }
        public string FacebookPageId { get; set; }
        public string PageName { get; set; }
        public string AccessToken { get; set; }
        public string ProfilePictureUrl { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
