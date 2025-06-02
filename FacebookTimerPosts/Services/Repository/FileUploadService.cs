using FacebookTimerPosts.Services.IRepository;

namespace FacebookTimerPosts.Services.Repository
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileUploadService> _logger;
        private readonly string _uploadPath;
        private readonly string _baseUrl;

        public FileUploadService(IConfiguration configuration, ILogger<FileUploadService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _uploadPath = _configuration["FileUpload:UploadPath"] ?? "uploads";
            _baseUrl = _configuration["FileUpload:BaseUrl"] ?? "https://localhost:7101";
        }

        public async Task<string> UploadUserAvatarAsync(IFormFile file, string userId)
        {
            try
            {
                var uploadsFolder = Path.Combine(_uploadPath, "avatars");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return $"{_baseUrl}/uploads/avatars/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading user avatar for user {UserId}", userId);
                throw;
            }
        }

        public async Task<string> UploadUserCoverPhotoAsync(IFormFile file, string userId)
        {
            try
            {
                var uploadsFolder = Path.Combine(_uploadPath, "covers");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return $"{_baseUrl}/uploads/covers/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading cover photo for user {UserId}", userId);
                throw;
            }
        }

        public async Task<string> UploadPostImageAsync(IFormFile file, string userId)
        {
            try
            {
                var uploadsFolder = Path.Combine(_uploadPath, "posts");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return $"{_baseUrl}/uploads/posts/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading post image for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string fileUrl)
        {
            try
            {
                var fileName = Path.GetFileName(fileUrl);
                var filePath = Path.Combine(_uploadPath, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {FileUrl}", fileUrl);
                return false;
            }
        }

        public async Task<long> GetFileSizeAsync(string fileUrl)
        {
            try
            {
                var fileName = Path.GetFileName(fileUrl);
                var filePath = Path.Combine(_uploadPath, fileName);

                if (File.Exists(filePath))
                {
                    var fileInfo = new FileInfo(filePath);
                    return fileInfo.Length;
                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file size for {FileUrl}", fileUrl);
                return 0;
            }
        }
    }
}
