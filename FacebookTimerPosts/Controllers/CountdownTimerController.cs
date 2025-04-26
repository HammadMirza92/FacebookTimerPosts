using FacebookTimerPosts.DTOs;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FacebookTimerPosts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountdownTimerController : ControllerBase
    {
        private readonly ICountdownTimerRepository _countdownTimerRepository;
        private readonly IPostRepository _postRepository;

        public CountdownTimerController(
            ICountdownTimerRepository countdownTimerRepository,
            IPostRepository postRepository)
        {
            _countdownTimerRepository = countdownTimerRepository;
            _postRepository = postRepository;
        }

        [Authorize]
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetTimerForPost(int postId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Verify post belongs to user
            if (!await _postRepository.PostBelongsToUserAsync(postId, userId))
            {
                return NotFound();
            }

            var timer = await _countdownTimerRepository.GetByPostIdAsync(postId);

            if (timer == null)
            {
                // Create new timer if one doesn't exist
                timer = await _countdownTimerRepository.CreateTimerForPostAsync(postId);
            }

            return Ok(new
            {
                id = timer.Id,
                postId = timer.PostId,
                publicId = timer.PublicId,
                isActive = timer.IsActive
            });
        }

        [HttpGet("public/{publicId}")]
        public async Task<IActionResult> GetPublicTimer(string publicId)
        {
            var timer = await _countdownTimerRepository.GetByPublicIdAsync(publicId);

            if (timer == null || !timer.IsActive || timer.Post == null)
            {
                return NotFound();
            }

            var post = timer.Post;

            var postPreview = new PostPreviewDto
            {
                Title = post.Title,
                Description = post.Description,
                EventDateTime = post.EventDateTime,
                CustomFontFamily = post.CustomFontFamily ?? post.Template.DefaultFontFamily,
                CustomPrimaryColor = post.CustomPrimaryColor ?? post.Template.PrimaryColor,
                ShowDays = post.ShowDays,
                ShowHours = post.ShowHours,
                ShowMinutes = post.ShowMinutes,
                ShowSeconds = post.ShowSeconds,
                BackgroundImageUrl = post.BackgroundImageUrl ?? post.Template.BackgroundImageUrl,
                HasOverlay = post.HasOverlay
            };

            return Ok(postPreview);
        }
    }
}
