using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository.Base;

namespace FacebookTimerPosts.Services.IRepository
{
    public interface ICountdownTimerRepository : IRepository<CountdownTimer>
    {
        Task<CountdownTimer> GetByPostIdAsync(int postId);
        Task<CountdownTimer> GetByPublicIdAsync(string publicId);
        Task<CountdownTimer> CreateTimerForPostAsync(int postId);
    }
}
