using FacebookTimerPosts.Enums;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository.Base;

namespace FacebookTimerPosts.Services.IRepository
{
    public interface ITemplateRepository : IRepository<Template>
    {
        Task<IEnumerable<Template>> GetTemplatesBySubscriptionTypeAsync(SubscriptionType subscriptionType);
        Task<IEnumerable<Template>> GetTemplatesByCategoryAsync(TemplateCategory category);
    }
}
