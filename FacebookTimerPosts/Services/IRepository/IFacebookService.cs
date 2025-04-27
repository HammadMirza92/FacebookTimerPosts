using FacebookTimerPosts.Models;

namespace FacebookTimerPosts.Services.IRepository
{
    public interface IFacebookService
    {
        Task<(bool Success, string PostId, string ErrorMessage)> PublishPostAsync(Post post, string imageUrl);
        Task<(bool Success, string PageAccessToken, string ErrorMessage)> ValidatePageAccessTokenAsync(FacebookPage page);
        Task<bool> DeletePostAsync(string postId, string pageAccessToken);
    }
}
