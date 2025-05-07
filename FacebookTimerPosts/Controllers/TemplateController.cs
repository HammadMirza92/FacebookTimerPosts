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
            //var templates = await _templateRepository.GetTemplatesForUserAsync(userId, subscriptionPlanId);
            var templates = await _templateRepository.GetAllAsync();
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
                    CreatedAt = template.CreatedAt,
                    HtmlTemplate = template.HtmlTemplate,
                    TemplateVariables = template.GetTemplateVariables().ToList()
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
                CreatedAt = template.CreatedAt,
                HtmlTemplate = template.HtmlTemplate,
                TemplateVariables = template.GetTemplateVariables().ToList()
            };

            return Ok(templateDto);
        }
        [HttpPost("preview/{id}")]
        public async Task<IActionResult> PreviewTemplate(int id, [FromBody] Dictionary<string, string> templateValues)
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

            // Apply template substitutions
            string htmlContent = template.HtmlTemplate;
            foreach (var variable in template.GetTemplateVariables())
            {
                if (templateValues.TryGetValue(variable, out string value))
                {
                    // Replace placeholders with actual values
                    htmlContent = htmlContent.Replace("{{" + variable + "}}", value);
                }
            }

            return Ok(new { Html = htmlContent });
        }
        [HttpPost]
        public async Task<IActionResult> CreateTemplate([FromBody] TemplateDto templateDto)
        {
            var template = new Template
            {
                Name = templateDto.Name,
                Description = templateDto.Description,
                BackgroundImageUrl = templateDto.BackgroundImageUrl,
                DefaultFontFamily = templateDto.DefaultFontFamily,
                PrimaryColor = templateDto.PrimaryColor,
                RequiresSubscription = templateDto.RequiresSubscription,
                MinimumSubscriptionPlanId = templateDto.MinimumSubscriptionPlanId,
                IsActive = true,
                HtmlTemplate = templateDto.HtmlTemplate
            };

            await _templateRepository.AddAsync(template);

            return CreatedAtAction(nameof(GetTemplate), new { id = template.Id }, template);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemplate(int id, [FromBody] TemplateDto templateDto)
        {
            var template = await _templateRepository.GetByIdAsync(id);

            if (template == null)
            {
                return NotFound();
            }

            template.Name = templateDto.Name;
            template.Description = templateDto.Description;
            template.BackgroundImageUrl = templateDto.BackgroundImageUrl;
            template.DefaultFontFamily = templateDto.DefaultFontFamily;
            template.PrimaryColor = templateDto.PrimaryColor;
            template.RequiresSubscription = templateDto.RequiresSubscription;
            template.MinimumSubscriptionPlanId = templateDto.MinimumSubscriptionPlanId;
            template.HtmlTemplate = templateDto.HtmlTemplate;
            template.UpdatedAt = DateTime.UtcNow;

            await _templateRepository.UpdateAsync(template);

            return NoContent();
        }
    }
}
