using FacebookTimerPosts.AppDbContext;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using FacebookTimerPosts.Services.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace FacebookTimerPosts.Services.Repository
{
    public class FacebookPageRepository : Repository<FacebookPage>, IFacebookPageRepository
    {
        private readonly ApplicationDbContext _context;

        public FacebookPageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IList<FacebookPage>> GetUserPagesAsync(string userId)
        {
            return await _context.FacebookPages
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<FacebookPage> GetUserPageByIdAsync(int id, string userId)
        {
            return await _context.FacebookPages
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        }
        public async Task<bool> PageBelongsToUserAsync(string pageId, string userId)
        {
            return await _context.FacebookPages
                .AnyAsync(p => p.PageId == pageId && p.UserId == userId);
        }

        public async Task<FacebookPage> LinkFacebookPageAsync(string userId, string pageId, string pageName, string pageAccessToken, DateTime expiryDate)
        {
            // Check if the page is already linked to this user
            var existingPage = await _context.FacebookPages
                .FirstOrDefaultAsync(p => p.PageAccessToken == pageAccessToken && p.UserId == userId);

            if (existingPage != null)
            {
                // Update the existing page with new token info
                existingPage.PageName = pageName;
                existingPage.PageAccessToken = pageAccessToken;
                existingPage.TokenExpiryDate = expiryDate;
                existingPage.IsActive = true;
                existingPage.UpdatedAt = DateTime.UtcNow;

                _context.FacebookPages.Update(existingPage);
                await _context.SaveChangesAsync();

                return existingPage;
            }
            else
            {
                // Create a new page link
                var newPage = new FacebookPage
                {
                    UserId = userId,
                    PageId = pageId,
                    PageName = pageName,
                    PageAccessToken = pageAccessToken,
                    TokenExpiryDate = expiryDate,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.FacebookPages.AddAsync(newPage);
                await _context.SaveChangesAsync();

                return newPage;
            }
        }
        public async Task<string> GetPageAccessTokenAsync(int pageId)
        {
            var page = await _context.FacebookPages.FindAsync(pageId);
            return page?.PageAccessToken;
        }
    }

}