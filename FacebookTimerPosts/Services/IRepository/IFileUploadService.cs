using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository.Base;

namespace FacebookTimerPosts.Services.IRepository
{
    public interface IFileUploadService 
    {
        Task<string> UploadUserAvatarAsync(IFormFile file, string userId);
        Task<string> UploadUserCoverPhotoAsync(IFormFile file, string userId);
        Task<string> UploadPostImageAsync(IFormFile file, string userId);
        Task<bool> DeleteFileAsync(string fileUrl);
        Task<long> GetFileSizeAsync(string fileUrl);
    }
}
