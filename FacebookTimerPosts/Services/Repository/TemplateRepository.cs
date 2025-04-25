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
        public TemplateRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Template>> GetTemplatesBySubscriptionTypeAsync(SubscriptionType subscriptionType)
        {
            return await _context.Templates
                .Where(t => t.MinimumSubscription <= subscriptionType)
                .ToListAsync();
        }

        public async Task<IEnumerable<Template>> GetTemplatesByCategoryAsync(TemplateCategory category)
        {
            return await _context.Templates
                .Where(t => t.Category == category)
                .ToListAsync();
        }
    }
}
