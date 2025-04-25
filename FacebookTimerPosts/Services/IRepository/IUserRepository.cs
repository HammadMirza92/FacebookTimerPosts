using FacebookTimerPosts.Enums;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository.Base;

namespace FacebookTimerPosts.Services.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserWithPagesAsync(int userId);
        Task<User> GetUserWithPostsAsync(int userId);
        Task<int> GetUserRemainingPostsForTodayAsync(int userId);
        Task<bool> UpdateUserSubscriptionAsync(int userId, SubscriptionType type, DateTime endDate);
        Task<bool> CheckUserExistsAsync(string username);
        Task<bool> CheckEmailExistsAsync(string email);
        Task<User> AuthenticateAsync(string username, string password);
        Task<User> RegisterAsync(string username, string email, string password);
    }
}
