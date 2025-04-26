using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository.Base;

namespace FacebookTimerPosts.Services.IRepository
{
    public interface IUserSubscriptionRepository : IRepository<UserSubscription>
    {
        Task<UserSubscription> GetCurrentSubscriptionAsync(string userId);
        Task<UserSubscription> AddSubscriptionAsync(string userId, int subscriptionPlanId, bool autoRenew, string paymentReferenceId);
        Task<int> CountActiveSubscriptionsAsync(string userId);
        Task<bool> HasActiveSubscriptionAsync(string userId);
        Task<bool> NeedsSubscriptionRenewalAlertAsync(string userId);
    }
}
