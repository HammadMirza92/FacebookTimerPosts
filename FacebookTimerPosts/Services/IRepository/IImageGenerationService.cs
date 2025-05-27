using FacebookTimerPosts.Models;

namespace FacebookTimerPosts.Services.IRepository
{
    public interface IImageGenerationService
    {
        Task<string> GenerateCountdownImageAsync(Post post, Template template);
        Task<byte[]> GenerateCountdownImageBytesAsync(Post post, Template template);
    }
}
