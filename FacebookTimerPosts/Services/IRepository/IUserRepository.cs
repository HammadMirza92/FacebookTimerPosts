using FacebookTimerPosts.Enums;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository.Base;
using Microsoft.AspNetCore.Identity;

namespace FacebookTimerPosts.Services.IRepository
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(string id);
        Task<User> GetUserByEmailAsync(string email);
        Task<IdentityResult> RegisterUserAsync(User user, string password);
        Task<SignInResult> LoginUserAsync(string email, string password);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<bool> UserExistsAsync(string email);
        Task<IList<string>> GetUserRolesAsync(User user);
        Task LogoutAsync();
        Task UpdateLastLoginDateAsync(User user);
    }
}
