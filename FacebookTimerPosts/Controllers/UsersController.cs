using AutoMapper;
using FacebookTimerPosts.DTOs;
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
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null) return NotFound();

            var userDto = _mapper.Map<UserDto>(user);

            // Calculate days until expiration if subscription exists
            if (user.SubscriptionEndDate.HasValue)
            {
                var daysLeft = (user.SubscriptionEndDate.Value - DateTime.UtcNow).Days;
                userDto.DaysUntilExpiration = Math.Max(0, daysLeft);
            }

            // Get remaining posts for today
            userDto.PostsRemainingToday = await _userRepository.GetUserRemainingPostsForTodayAsync(id);

            return userDto;
        }

        [HttpGet("pages")]
        public async Task<ActionResult<IEnumerable<FacebookPageDto>>> GetUserPages()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = await _userRepository.GetUserWithPagesAsync(userId);

            if (user == null) return NotFound();

            return Ok(_mapper.Map<IEnumerable<FacebookPageDto>>(user.LinkedPages));
        }

        [HttpPut("subscription")]
        public async Task<ActionResult> UpdateSubscription(SubscriptionUpdateDto subscriptionDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var success = await _userRepository.UpdateUserSubscriptionAsync(
                userId,
                subscriptionDto.SubscriptionType,
                subscriptionDto.EndDate);

            if (!success) return BadRequest("Failed to update subscription");

            return NoContent();
        }
    }
}
