using FacebookTimerPosts.DTOs;
using FacebookTimerPosts.Enums;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository.Base;
using Microsoft.AspNetCore.Identity;

namespace FacebookTimerPosts.Services.IRepository
{
    public interface IUserRepository
    {
        // Basic user operations
        Task<User> GetUserByIdAsync(string id);
        Task<User> GetUserByEmailAsync(string email);
        Task<IdentityResult> RegisterUserAsync(User user, string password);
        Task<SignInResult> LoginUserAsync(string email, string password);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<bool> UserExistsAsync(string email);
        Task<IList<string>> GetUserRolesAsync(User user);
        Task LogoutAsync();
        Task UpdateLastLoginDateAsync(User user);

        // Additional methods that might be useful
        Task<IdentityResult> CreateUserAsync(User user);
        Task<IdentityResult> UpdateUserAsync(User user);
        Task<User> FindUserByEmailAsync(string email);

        // External login support methods (for the controller to use)
        Task<User> GetUserByLoginAsync(string loginProvider, string providerKey);
        Task<IdentityResult> AddExternalLoginAsync(User user, UserLoginInfo loginInfo);
        Task<IList<UserLoginInfo>> GetLoginsAsync(User user);
        Task<bool> HasExternalLoginAsync(User user, string provider, string providerKey);
    }
}