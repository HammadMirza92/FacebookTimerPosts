using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository.Base;
using FacebookTimerPosts.Services.Repository;

namespace FacebookTimerPosts.Services.IRepository
{
    public interface IFacebookPageRepository : IRepository<FacebookPage>
    {
        Task<IList<FacebookPage>> GetUserPagesAsync(string userId);
        Task<FacebookPage> GetUserPageByIdAsync(int id, string userId);
        Task<FacebookPage> LinkFacebookPageAsync(string userId, string pageId, string pageName, string accessToken, DateTime expiryDate);
        Task<bool> PageBelongsToUserAsync(int pageId, string userId);
    }
}
