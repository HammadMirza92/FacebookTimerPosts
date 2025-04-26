using FacebookTimerPosts.AppDbContext;
using FacebookTimerPosts.Enums;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using FacebookTimerPosts.Services.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace FacebookTimerPosts.Services.Repository
{
    public class TemplateRepository : Repository<Template>, ITemplateRepository
    {
        private readonly ApplicationDbContext _db;

        public TemplateRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<int> CountUserAvailableTemplatesAsync(string userId, int? subscriptionPlanId)
        {
            var query = _db.Templates.Where(t => t.IsActive);

            // If user has subscription, include templates available for their plan
            if (subscriptionPlanId.HasValue)
            {
                query = query.Where(t => !t.RequiresSubscription ||
                                        (t.MinimumSubscriptionPlanId.HasValue &&
                                         t.MinimumSubscriptionPlanId.Value <= subscriptionPlanId.Value));
            }
            else
            {
                // Free users only get templates that don't require subscription
                query = query.Where(t => !t.RequiresSubscription);
            }

            return await query.CountAsync();
        }

        public async Task<IList<Template>> GetTemplatesForUserAsync(string userId, int? subscriptionPlanId)
        {
            var query = _db.Templates
                .Include(t => t.MinimumSubscriptionPlan)
                .Where(t => t.IsActive);

            // If user has subscription, include templates available for their plan
            if (subscriptionPlanId.HasValue)
            {
                query = query.Where(t => !t.RequiresSubscription ||
                                        (t.MinimumSubscriptionPlanId.HasValue &&
                                         t.MinimumSubscriptionPlanId.Value <= subscriptionPlanId.Value));
            }
            else
            {
                // Free users only get templates that don't require subscription
                query = query.Where(t => !t.RequiresSubscription);
            }

            return await query.OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<bool> IsTemplateAccessibleToUserAsync(int templateId, string userId, int? subscriptionPlanId)
        {
            var template = await _db.Templates.FindAsync(templateId);

            if (template == null || !template.IsActive)
            {
                return false;
            }

            // Free templates are accessible to all
            if (!template.RequiresSubscription)
            {
                return true;
            }

            // If template requires subscription but user doesn't have one
            if (!subscriptionPlanId.HasValue)
            {
                return false;
            }

            // Check if user's subscription plan level is sufficient for the template
            return !template.MinimumSubscriptionPlanId.HasValue ||
                   template.MinimumSubscriptionPlanId.Value <= subscriptionPlanId.Value;
        }
    }
}
