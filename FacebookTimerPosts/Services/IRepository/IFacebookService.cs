using FacebookTimerPosts.Models;

namespace FacebookTimerPosts.Services.IRepository
{
    public interface IFacebookService
    {
        Task<FacebookPostResult> PublishPostAsync(Post post, string imageUrl = null);
        Task<(bool Success, string PageAccessToken, string ErrorMessage)> ValidatePageAccessTokenAsync(FacebookPage page);
        Task<bool> DeletePostAsync(string postId, string pageAccessToken);
        Task<bool> ValidateAccessTokenAsync(string accessToken);
        Task<bool> PostExistsAsync(string postId, string pageAccessToken);

    }
}
