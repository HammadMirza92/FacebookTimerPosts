using FacebookTimerPosts.AppDbContext;
using FacebookTimerPosts.Enums;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using FacebookTimerPosts.Services.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace FacebookTimerPosts.Services.Repository
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Post>> GetUserPostsAsync(int userId)
        {
            return await _context.Posts
                .Include(p => p.FacebookPage)
                .Include(p => p.Template)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPagePostsAsync(int pageId)
        {
            return await _context.Posts
                .Include(p => p.Template)
                .Where(p => p.FacebookPageId == pageId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Post> GetPostWithDetailsAsync(int postId)
        {
            return await _context.Posts
                .Include(p => p.FacebookPage)
                .Include(p => p.Template)
                .FirstOrDefaultAsync(p => p.Id == postId);
        }

        public async Task<IEnumerable<Post>> GetActiveCountdownsAsync()
        {
            return await _context.Posts
                .Where(p => p.Status == PostStatus.Published && p.TargetDate > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<int> GetUserPostCountForTodayAsync(int userId)
        {
            return await _context.Posts
                .CountAsync(p => p.UserId == userId &&
                          p.CreatedAt.Date == DateTime.UtcNow.Date);
        }

        public async Task<Post> GetPostByCountdownUrlAsync(string urlSegment)
        {
            return await _context.Posts
                .Include(p => p.Template)
                .FirstOrDefaultAsync(p => p.CountdownUrl.EndsWith(urlSegment));
        }
    }
}
