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
        private readonly ApplicationDbContext _db;

        public PostRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<int> CountUserActivePosts(string userId)
        {
            return await _db.Posts
                .CountAsync(p => p.UserId == userId &&
                           (p.Status == PostStatus.Draft || p.Status == PostStatus.Scheduled || p.Status == PostStatus.Published));
        }

        public async Task<IList<Post>> GetPagePostsAsync(int facebookPageId, string userId)
        {
            return await _db.Posts
                .Include(p => p.Template)
                .Include(p => p.FacebookPage)
                .Where(p => p.FacebookPageId == facebookPageId && p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IList<Post>> GetScheduledPostsForPublishingAsync()
        {
            var now = DateTime.UtcNow;

            return await _db.Posts
                .Include(p => p.FacebookPage)
                .Include(p => p.Template)
                .Where(p => p.Status == PostStatus.Scheduled &&
                           p.ScheduledFor.HasValue &&
                           p.ScheduledFor.Value <= now)
                .ToListAsync();
        }

        public async Task<Post> GetUserPostByIdAsync(int id, string userId)
        {
            return await _db.Posts
                .Include(p => p.Template)
                .Include(p => p.FacebookPage)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        }

        public async Task<IList<Post>> GetUserPostsAsync(string userId)
        {
            return await _db.Posts
                .Include(p => p.Template)
                .Include(p => p.FacebookPage)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> PostBelongsToUserAsync(int postId, string userId)
        {
            return await _db.Posts
                .AnyAsync(p => p.Id == postId && p.UserId == userId);
        }

        public async Task UpdatePostStatusAsync(int postId, PostStatus status, string facebookPostId = null)
        {
            var post = await _db.Posts.FindAsync(postId);
            if (post != null)
            {
                post.Status = status;
                post.UpdatedAt = DateTime.UtcNow;

                if (status == PostStatus.Published)
                {
                    post.PublishedAt = DateTime.UtcNow;

                    if (!string.IsNullOrEmpty(facebookPostId))
                    {
                        post.FacebookPostId = facebookPostId;
                    }
                }

                _db.Posts.Update(post);
                await _db.SaveChangesAsync();
            }
        }
    }
}
