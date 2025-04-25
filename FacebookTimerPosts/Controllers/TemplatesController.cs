using AutoMapper;
using FacebookTimerPosts.DTOs;
using FacebookTimerPosts.Enums;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FacebookTimerPosts.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TemplatesController : ControllerBase
    {
        private readonly ITemplateRepository _templateRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public TemplatesController(
            ITemplateRepository templateRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _templateRepository = templateRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TemplateDto>>> GetTemplates()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null) return NotFound();

            var templates = await _templateRepository.GetTemplatesBySubscriptionTypeAsync(user.SubscriptionType);

            return Ok(_mapper.Map<IEnumerable<TemplateDto>>(templates));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TemplateDto>> GetTemplate(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null) return NotFound();

            var template = await _templateRepository.GetByIdAsync(id);

            if (template == null) return NotFound();

            // Check if user has access to this template
            if (template.MinimumSubscription > user.SubscriptionType)
                return Forbid();

            return _mapper.Map<TemplateDto>(template);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<TemplateDto>>> GetTemplatesByCategory(string category)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null) return NotFound();

            if (!Enum.TryParse<TemplateCategory>(category, true, out var templateCategory))
                return BadRequest("Invalid category");

            var templates = await _templateRepository.GetTemplatesByCategoryAsync(templateCategory);

            // Filter templates by user's subscription level
            templates = templates.Where(t => t.MinimumSubscription <= user.SubscriptionType);

            return Ok(_mapper.Map<IEnumerable<TemplateDto>>(templates));
        }
    }
}
