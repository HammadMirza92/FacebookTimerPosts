using FacebookTimerPosts.AppDbContext;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using FacebookTimerPosts.Services.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace FacebookTimerPosts.Services.Repository
{
    public class SubscriptionPlanRepository : Repository<SubscriptionPlan>, ISubscriptionPlanRepository
    {
        private readonly ApplicationDbContext _db;

        public SubscriptionPlanRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IList<SubscriptionPlan>> GetActiveSubscriptionPlansAsync()
        {
            return await _db.SubscriptionPlans
                .Where(sp => sp.IsActive)
                .OrderBy(sp => sp.Price)
                .ToListAsync();
        }

        public async Task<bool> IsValidSubscriptionPlanIdAsync(int subscriptionPlanId)
        {
            return await _db.SubscriptionPlans
                .AnyAsync(sp => sp.Id == subscriptionPlanId && sp.IsActive);
        }
    }
}
