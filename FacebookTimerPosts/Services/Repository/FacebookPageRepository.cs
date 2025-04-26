using FacebookTimerPosts.AppDbContext;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using FacebookTimerPosts.Services.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace FacebookTimerPosts.Services.Repository
{
    public class FacebookPageRepository : Repository<FacebookPage>, IFacebookPageRepository
    {
        private readonly ApplicationDbContext _db;

        public FacebookPageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<FacebookPage> GetUserPageByIdAsync(int id, string userId)
        {
            return await _db.FacebookPages
                .FirstOrDefaultAsync(fp => fp.Id == id && fp.UserId == userId && fp.IsActive);
        }

        public async Task<IList<FacebookPage>> GetUserPagesAsync(string userId)
        {
            return await _db.FacebookPages
                .Where(fp => fp.UserId == userId && fp.IsActive)
                .OrderBy(fp => fp.PageName)
                .ToListAsync();
        }

        public async Task<FacebookPage> LinkFacebookPageAsync(string userId, string pageId, string pageName, string accessToken, DateTime expiryDate)
        {
            // Check if page already exists for this user
            var existingPage = await _db.FacebookPages
                .FirstOrDefaultAsync(fp => fp.UserId == userId && fp.PageId == pageId);

            if (existingPage != null)
            {
                // Update existing page
                existingPage.PageName = pageName;
                existingPage.PageAccessToken = accessToken;
                existingPage.TokenExpiryDate = expiryDate;
                existingPage.IsActive = true;
                existingPage.UpdatedAt = DateTime.UtcNow;

                _db.FacebookPages.Update(existingPage);
                await _db.SaveChangesAsync();

                return existingPage;
            }

            // Create new page
            var newPage = new FacebookPage
            {
                UserId = userId,
                PageId = pageId,
                PageName = pageName,
                PageAccessToken = accessToken,
                TokenExpiryDate = expiryDate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _db.FacebookPages.AddAsync(newPage);
            await _db.SaveChangesAsync();

            return newPage;
        }

        public async Task<bool> PageBelongsToUserAsync(int pageId, string userId)
        {
            return await _db.FacebookPages
                .AnyAsync(fp => fp.Id == pageId && fp.UserId == userId && fp.IsActive);
        }
    }
}
