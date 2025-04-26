using FacebookTimerPosts.AppDbContext;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using FacebookTimerPosts.Services.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace FacebookTimerPosts.Services.Repository
{
    public class UserSubscriptionRepository : Repository<UserSubscription>, IUserSubscriptionRepository
    {
        private readonly ApplicationDbContext _db;

        public UserSubscriptionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<UserSubscription> AddSubscriptionAsync(string userId, int subscriptionPlanId, bool autoRenew, string paymentReferenceId)
        {
            var subscriptionPlan = await _db.SubscriptionPlans.FindAsync(subscriptionPlanId);
            if (subscriptionPlan == null)
            {
                return null;
            }

            var userSubscription = new UserSubscription
            {
                UserId = userId,
                SubscriptionPlanId = subscriptionPlanId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(subscriptionPlan.DurationInDays),
                IsActive = true,
                AutoRenew = autoRenew,
                PaymentReferenceId = paymentReferenceId,
                CreatedAt = DateTime.UtcNow
            };

            await _db.UserSubscriptions.AddAsync(userSubscription);
            await _db.SaveChangesAsync();

            return userSubscription;
        }

        public async Task<int> CountActiveSubscriptionsAsync(string userId)
        {
            return await _db.UserSubscriptions
                .CountAsync(us => us.UserId == userId && us.IsActive && us.EndDate > DateTime.UtcNow);
        }

        public async Task<UserSubscription> GetCurrentSubscriptionAsync(string userId)
        {
            return await _db.UserSubscriptions
                .Include(us => us.SubscriptionPlan)
                .Where(us => us.UserId == userId && us.IsActive && us.EndDate > DateTime.UtcNow)
                .OrderByDescending(us => us.EndDate)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasActiveSubscriptionAsync(string userId)
        {
            return await _db.UserSubscriptions
                .AnyAsync(us => us.UserId == userId && us.IsActive && us.EndDate > DateTime.UtcNow);
        }

        public async Task<bool> NeedsSubscriptionRenewalAlertAsync(string userId)
        {
            var now = DateTime.UtcNow;
            var alertThreshold = now.AddDays(4); // Alert 4 days before expiry

            return await _db.UserSubscriptions
                .AnyAsync(us => us.UserId == userId && us.IsActive &&
                           us.EndDate > now && us.EndDate <= alertThreshold);
        }
    }
}
