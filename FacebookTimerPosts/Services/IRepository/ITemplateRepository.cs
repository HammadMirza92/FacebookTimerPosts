using FacebookTimerPosts.Enums;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository.Base;

namespace FacebookTimerPosts.Services.IRepository
{
    public interface ITemplateRepository : IRepository<Template>
    {
        Task<IList<Template>> GetTemplatesForUserAsync(string userId, int? subscriptionPlanId);
        Task<int> CountUserAvailableTemplatesAsync(string userId, int? subscriptionPlanId);
        Task<bool> IsTemplateAccessibleToUserAsync(int templateId, string userId, int? subscriptionPlanId);
    }
}
