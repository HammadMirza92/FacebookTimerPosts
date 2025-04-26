using FacebookTimerPosts.AppDbContext;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using FacebookTimerPosts.Services.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace FacebookTimerPosts.Services.Repository
{
    public class CountdownTimerRepository : Repository<CountdownTimer>, ICountdownTimerRepository
    {
        private readonly ApplicationDbContext _db;

        public CountdownTimerRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<CountdownTimer> CreateTimerForPostAsync(int postId)
        {
            var existingTimer = await _db.CountdownTimers
                .FirstOrDefaultAsync(ct => ct.PostId == postId);

            if (existingTimer != null)
            {
                return existingTimer;
            }

            var newTimer = new CountdownTimer
            {
                PostId = postId,
                PublicId = Guid.NewGuid().ToString(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _db.CountdownTimers.AddAsync(newTimer);
            await _db.SaveChangesAsync();

            return newTimer;
        }

        public async Task<CountdownTimer> GetByPostIdAsync(int postId)
        {
            return await _db.CountdownTimers
                .Include(ct => ct.Post)
                .ThenInclude(p => p.Template)
                .FirstOrDefaultAsync(ct => ct.PostId == postId && ct.IsActive);
        }

        public async Task<CountdownTimer> GetByPublicIdAsync(string publicId)
        {
            return await _db.CountdownTimers
                .Include(ct => ct.Post)
                .ThenInclude(p => p.Template)
                .FirstOrDefaultAsync(ct => ct.PublicId == publicId && ct.IsActive);
        }
    }
}
