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
        private const string DEACTIVATED_TOKEN = "DEACTIVATED_TOKEN_DO_NOT_USE";

        public FacebookPageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IList<FacebookPage>> GetUserPagesAsync(string userId)
        {
            // First check and update status of any expired pages
            var now = DateTime.UtcNow;
            var expiredPages = await _context.FacebookPages
                .Where(p => p.UserId == userId && p.IsActive && p.TokenExpiryDate <= now)
                .ToListAsync();

            if (expiredPages.Any())
            {
                foreach (var page in expiredPages)
                {
                    page.IsActive = false;
                    page.UpdatedAt = now;
                }
                await _context.SaveChangesAsync();
            }

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
            // Check if the page is already linked to this user based on pageId
            var existingPage = await _context.FacebookPages
                .FirstOrDefaultAsync(p => p.PageId == pageId && p.UserId == userId);

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
            if (page != null && page.IsActive && page.PageAccessToken != DEACTIVATED_TOKEN)
            {
                return page.PageAccessToken;
            }

            return null;
        }

        public async Task<bool> DeleteFacebookPageByPageIdAsync(string pageId, string userId)
        {
            try
            {
                // Find the page by Facebook pageId (not the database id) and user id
                var page = await _context.FacebookPages
                    .FirstOrDefaultAsync(p => p.PageId == pageId && p.UserId == userId);

                if (page == null)
                {
                    return false;
                }

                // Check if there are any posts associated with this page
                bool hasAssociatedPosts = await _context.Posts
                    .AnyAsync(p => p.FacebookPageId == page.Id);

                if (hasAssociatedPosts)
                {
                    // Instead of deleting the page (which would violate the foreign key constraint),
                    // we mark it as inactive and clear the access token
                    page.IsActive = false;
                    page.PageAccessToken = DEACTIVATED_TOKEN; // Clear token for security
                    page.UpdatedAt = DateTime.UtcNow;

                    _context.FacebookPages.Update(page);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // No posts associated, safe to delete
                    _context.FacebookPages.Remove(page);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Add a new method to check if a Facebook page is linked
        public async Task<bool> IsPageLinkedAsync(string pageId)
        {
            var page = await _context.FacebookPages
                .FirstOrDefaultAsync(p => p.PageId == pageId);

            // A page is considered linked if it's active and has a valid token
            return page != null && page.IsActive && page.PageAccessToken != DEACTIVATED_TOKEN;
        }

        // Get Facebook page by pageId
        public async Task<FacebookPage> GetPageByPageIdAsync(string pageId)
        {
            return await _context.FacebookPages
                .FirstOrDefaultAsync(p => p.PageId == pageId);
        }
    }
}