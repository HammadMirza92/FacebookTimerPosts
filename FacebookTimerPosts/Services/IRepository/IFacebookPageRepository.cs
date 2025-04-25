using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository.Base;
using FacebookTimerPosts.Services.Repository;

namespace FacebookTimerPosts.Services.IRepository
{
    public interface IFacebookPageRepository : IRepository<FacebookPage>
    {
        Task<IEnumerable<FacebookPage>> GetUserPagesAsync(int userId);
        Task<FacebookPage> GetPageWithPostsAsync(int pageId);
        Task<bool> LinkPageToUserAsync(FacebookPage page, int userId);
        Task<bool> UnlinkPageFromUserAsync(int pageId, int userId);
        Task<FacebookPage> GetPageByFacebookIdAsync(string facebookPageId);
        Task<PageDetails> GetPageDetailsAsync(string pageId, string accessToken);
        Task<string> CreatePostAsync(string pageId, string accessToken, string message);
    }
}
