using FacebookTimerPosts.Enums;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository.Base;

namespace FacebookTimerPosts.Services.IRepository
{
    public interface IPostRepository : IRepository<Post>
    {
        Task<IList<Post>> GetUserPostsAsync(string userId);
        Task<IList<Post>> GetPagePostsAsync(int facebookPageId, string userId);
        Task<Post> GetUserPostByIdAsync(int id, string userId);
        Task<IList<Post>> GetScheduledPostsForPublishingAsync();
        Task<int> CountUserActivePosts(string userId);
        Task UpdatePostStatusAsync(int postId, PostStatus status, string facebookPostId = null);
        Task<bool> PostBelongsToUserAsync(int postId, string userId);
        Task<IList<Post>> GetDueScheduledPosts(DateTime now);
        Task<IList<Post>> GetPostsDueForRefresh(DateTime now);
        Task UpdatePostRefreshAsync(int id, string newFacebookPostId, DateTime nextRefreshTime);

    }
}
