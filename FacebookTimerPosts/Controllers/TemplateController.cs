using FacebookTimerPosts.DTOs;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FacebookTimerPosts.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateRepository _templateRepository;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;

        public TemplateController(
            ITemplateRepository templateRepository,
            IUserSubscriptionRepository userSubscriptionRepository)
        {
            _templateRepository = templateRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplates()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userSubscription = await _userSubscriptionRepository.GetCurrentSubscriptionAsync(userId);
            int? subscriptionPlanId = userSubscription?.SubscriptionPlanId;

            var templates = await _templateRepository.GetTemplatesForUserAsync(userId, subscriptionPlanId);

            var templatesDto = new List<TemplateDto>();
            foreach (var template in templates)
            {
                templatesDto.Add(new TemplateDto
                {
                    Id = template.Id,
                    Name = template.Name,
                    Description = template.Description,
                    BackgroundImageUrl = template.BackgroundImageUrl,
                    DefaultFontFamily = template.DefaultFontFamily,
                    PrimaryColor = template.PrimaryColor,
                    RequiresSubscription = template.RequiresSubscription,
                    MinimumSubscriptionPlanId = template.MinimumSubscriptionPlanId,
                    MinimumSubscriptionPlanName = template.MinimumSubscriptionPlan?.Name,
                    IsActive = template.IsActive,
                    CreatedAt = template.CreatedAt
                });
            }

            return Ok(templatesDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTemplate(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userSubscription = await _userSubscriptionRepository.GetCurrentSubscriptionAsync(userId);
            int? subscriptionPlanId = userSubscription?.SubscriptionPlanId;

            // Check if template is accessible to user
            if (!await _templateRepository.IsTemplateAccessibleToUserAsync(id, userId, subscriptionPlanId))
            {
                return NotFound();
            }

            var template = await _templateRepository.GetByIdAsync(id);

            var templateDto = new TemplateDto
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                BackgroundImageUrl = template.BackgroundImageUrl,
                DefaultFontFamily = template.DefaultFontFamily,
                PrimaryColor = template.PrimaryColor,
                RequiresSubscription = template.RequiresSubscription,
                MinimumSubscriptionPlanId = template.MinimumSubscriptionPlanId,
                MinimumSubscriptionPlanName = template.MinimumSubscriptionPlan?.Name,
                IsActive = template.IsActive,
                CreatedAt = template.CreatedAt
            };

            return Ok(templateDto);
        }
    }
}
