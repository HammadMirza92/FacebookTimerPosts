using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository.Base;

namespace FacebookTimerPosts.Services.IRepository
{
    public interface ISubscriptionPlanRepository : IRepository<SubscriptionPlan>
    {
        Task<IList<SubscriptionPlan>> GetActiveSubscriptionPlansAsync();
        Task<bool> IsValidSubscriptionPlanIdAsync(int subscriptionPlanId);
    }
}
