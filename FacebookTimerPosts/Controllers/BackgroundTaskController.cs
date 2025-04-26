using FacebookTimerPosts.Enums;
using FacebookTimerPosts.Services.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace FacebookTimerPosts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackgroundTaskController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly IFacebookPageRepository _facebookPageRepository;
        private readonly ILogger<BackgroundTaskController> _logger;

        public BackgroundTaskController(
            IPostRepository postRepository,
            IFacebookPageRepository facebookPageRepository,
            ILogger<BackgroundTaskController> logger)
        {
            _postRepository = postRepository;
            _facebookPageRepository = facebookPageRepository;
            _logger = logger;
        }

        [HttpPost("process-scheduled-posts")]
        public async Task<IActionResult> ProcessScheduledPosts([FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            // Validate API key (this should be stored securely and compared securely)
            if (apiKey != "your-secure-api-key-for-background-tasks")
            {
                return Unauthorized();
            }

            try
            {
                var scheduledPosts = await _postRepository.GetScheduledPostsForPublishingAsync();
                int processedCount = 0;
                int failedCount = 0;

                foreach (var post in scheduledPosts)
                {
                    try
                    {
                        // In a real application, you would call Facebook API to publish the post
                        // For this example, we'll just update the status
                        string fakePostId = "facebook_scheduled_" + Guid.NewGuid().ToString();

                        await _postRepository.UpdatePostStatusAsync(post.Id, PostStatus.Published, fakePostId);
                        processedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to publish scheduled post {PostId}", post.Id);
                        await _postRepository.UpdatePostStatusAsync(post.Id, PostStatus.Failed);
                        failedCount++;
                    }
                }

                return Ok(new
                {
                    scheduledPostsCount = scheduledPosts.Count,
                    processedCount = processedCount,
                    failedCount = failedCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing scheduled posts");
                return StatusCode(500, "An error occurred while processing scheduled posts");
            }
        }
    }
}
