using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository.Base;

namespace FacebookTimerPosts.Services.IRepository
{
    public interface IPostRepository : IRepository<Post>
    {
        Task<IEnumerable<Post>> GetUserPostsAsync(int userId);
        Task<IEnumerable<Post>> GetPagePostsAsync(int pageId);
        Task<Post> GetPostWithDetailsAsync(int postId);
        Task<IEnumerable<Post>> GetActiveCountdownsAsync();
        Task<int> GetUserPostCountForTodayAsync(int userId);
        Task<Post> GetPostByCountdownUrlAsync(string urlSegment);
    }
}
