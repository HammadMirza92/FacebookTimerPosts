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
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly IFacebookPageRepository _facebookPageRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ICountdownTimerRepository _countdownTimerRepository;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly ILogger<PostController> _logger;

        public PostController(
            IPostRepository postRepository,
            IFacebookPageRepository facebookPageRepository,
            ITemplateRepository templateRepository,
            ICountdownTimerRepository countdownTimerRepository,
            IUserSubscriptionRepository userSubscriptionRepository,
            ILogger<PostController> logger)
        {
            _postRepository = postRepository;
            _facebookPageRepository = facebookPageRepository;
            _templateRepository = templateRepository;
            _countdownTimerRepository = countdownTimerRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPosts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var posts = await _postRepository.GetUserPostsAsync(userId);

            var postsDto = new List<PostDto>();
            foreach (var post in posts)
            {
                var countdownTimer = await _countdownTimerRepository.GetByPostIdAsync(post.Id);

                postsDto.Add(new PostDto
                {
                    Id = post.Id,
                    FacebookPageId = post.FacebookPageId,
                    FacebookPageName = post.FacebookPage.PageName,
                    TemplateId = post.TemplateId,
                    TemplateName = post.Template.Name,
                    Title = post.Title,
                    Description = post.Description,
                    EventDateTime = post.EventDateTime,
                    CustomFontFamily = post.CustomFontFamily,
                    CustomPrimaryColor = post.CustomPrimaryColor,
                    ShowDays = post.ShowDays,
                    ShowHours = post.ShowHours,
                    ShowMinutes = post.ShowMinutes,
                    ShowSeconds = post.ShowSeconds,
                    BackgroundImageUrl = post.BackgroundImageUrl,
                    HasOverlay = post.HasOverlay,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt,
                    FacebookPostId = post.FacebookPostId,
                    Status = post.Status,
                    PublishedAt = post.PublishedAt,
                    ScheduledFor = post.ScheduledFor,
                    RefreshIntervalInMinutes = post.RefreshIntervalInMinutes,
                    CountdownPublicId = countdownTimer?.PublicId
                });
            }

            return Ok(postsDto);
        }

        [HttpGet("page/{pageId}")]
        public async Task<IActionResult> GetPagePosts(int pageId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Verify page belongs to user
            if (!await _facebookPageRepository.PageBelongsToUserAsync(pageId, userId))
            {
                return NotFound();
            }

            var posts = await _postRepository.GetPagePostsAsync(pageId, userId);

            var postsDto = new List<PostDto>();
            foreach (var post in posts)
            {
                var countdownTimer = await _countdownTimerRepository.GetByPostIdAsync(post.Id);

                postsDto.Add(new PostDto
                {
                    Id = post.Id,
                    FacebookPageId = post.FacebookPageId,
                    FacebookPageName = post.FacebookPage.PageName,
                    TemplateId = post.TemplateId,
                    TemplateName = post.Template.Name,
                    Title = post.Title,
                    Description = post.Description,
                    EventDateTime = post.EventDateTime,
                    CustomFontFamily = post.CustomFontFamily,
                    CustomPrimaryColor = post.CustomPrimaryColor,
                    ShowDays = post.ShowDays,
                    ShowHours = post.ShowHours,
                    ShowMinutes = post.ShowMinutes,
                    ShowSeconds = post.ShowSeconds,
                    BackgroundImageUrl = post.BackgroundImageUrl,
                    HasOverlay = post.HasOverlay,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt,
                    FacebookPostId = post.FacebookPostId,
                    Status = post.Status,
                    PublishedAt = post.PublishedAt,
                    ScheduledFor = post.ScheduledFor,
                    RefreshIntervalInMinutes = post.RefreshIntervalInMinutes,
                    CountdownPublicId = countdownTimer?.PublicId
                });
            }

            return Ok(postsDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var post = await _postRepository.GetUserPostByIdAsync(id, userId);

            if (post == null)
            {
                return NotFound();
            }

            var countdownTimer = await _countdownTimerRepository.GetByPostIdAsync(post.Id);

            var postDto = new PostDto
            {
                Id = post.Id,
                FacebookPageId = post.FacebookPageId,
                FacebookPageName = post.FacebookPage.PageName,
                TemplateId = post.TemplateId,
                TemplateName = post.Template.Name,
                Title = post.Title,
                Description = post.Description,
                EventDateTime = post.EventDateTime,
                CustomFontFamily = post.CustomFontFamily,
                CustomPrimaryColor = post.CustomPrimaryColor,
                ShowDays = post.ShowDays,
                ShowHours = post.ShowHours,
                ShowMinutes = post.ShowMinutes,
                ShowSeconds = post.ShowSeconds,
                BackgroundImageUrl = post.BackgroundImageUrl,
                HasOverlay = post.HasOverlay,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                FacebookPostId = post.FacebookPostId,
                Status = post.Status,
                PublishedAt = post.PublishedAt,
                ScheduledFor = post.ScheduledFor,
                RefreshIntervalInMinutes = post.RefreshIntervalInMinutes,
                CountdownPublicId = countdownTimer?.PublicId
            };

            return Ok(postDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto createPostDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Verify page belongs to user
            if (!await _facebookPageRepository.PageBelongsToUserAsync(createPostDto.FacebookPageId, userId))
            {
                return BadRequest("Facebook page not found or doesn't belong to you");
            }

            // Verify template is accessible to user
            var userSubscription = await _userSubscriptionRepository.GetCurrentSubscriptionAsync(userId);
            int? subscriptionPlanId = userSubscription?.SubscriptionPlanId;

            if (!await _templateRepository.IsTemplateAccessibleToUserAsync(createPostDto.TemplateId, userId, subscriptionPlanId))
            {
                return BadRequest("Template not found or not accessible with your subscription");
            }

            // Check posts limit based on subscription
            var postsCount = await _postRepository.CountUserActivePosts(userId);
            var maxPosts = userSubscription != null ? userSubscription.SubscriptionPlan.MaxPosts : 1; // Free tier limit

            if (maxPosts > 0 && postsCount >= maxPosts)
            {
                return BadRequest("You have reached the maximum number of active posts allowed by your subscription");
            }

            var post = new Post
            {
                UserId = userId,
                FacebookPageId = createPostDto.FacebookPageId,
                TemplateId = createPostDto.TemplateId,
                Title = createPostDto.Title,
                Description = createPostDto.Description,
                EventDateTime = createPostDto.EventDateTime,
                CustomFontFamily = createPostDto.CustomFontFamily,
                CustomPrimaryColor = createPostDto.CustomPrimaryColor,
                ShowDays = createPostDto.ShowDays,
                ShowHours = createPostDto.ShowHours,
                ShowMinutes = createPostDto.ShowMinutes,
                ShowSeconds = createPostDto.ShowSeconds,
                BackgroundImageUrl = createPostDto.BackgroundImageUrl,
                HasOverlay = createPostDto.HasOverlay,
                CreatedAt = DateTime.UtcNow,
                Status = PostStatus.Draft,
                RefreshIntervalInMinutes = createPostDto.RefreshIntervalInMinutes
            };

            if (createPostDto.ScheduledFor.HasValue)
            {
                post.ScheduledFor = createPostDto.ScheduledFor;
                post.Status = PostStatus.Scheduled;
            }

            await _postRepository.AddAsync(post);

            // Create countdown timer
            var countdownTimer = await _countdownTimerRepository.CreateTimerForPostAsync(post.Id);

            var postDto = new PostDto
            {
                Id = post.Id,
                FacebookPageId = post.FacebookPageId,
                TemplateId = post.TemplateId,
                Title = post.Title,
                Description = post.Description,
                EventDateTime = post.EventDateTime,
                CustomFontFamily = post.CustomFontFamily,
                CustomPrimaryColor = post.CustomPrimaryColor,
                ShowDays = post.ShowDays,
                ShowHours = post.ShowHours,
                ShowMinutes = post.ShowMinutes,
                ShowSeconds = post.ShowSeconds,
                BackgroundImageUrl = post.BackgroundImageUrl,
                HasOverlay = post.HasOverlay,
                CreatedAt = post.CreatedAt,
                Status = post.Status,
                ScheduledFor = post.ScheduledFor,
                RefreshIntervalInMinutes = post.RefreshIntervalInMinutes,
                CountdownPublicId = countdownTimer.PublicId
            };

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, postDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostDto updatePostDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var post = await _postRepository.GetUserPostByIdAsync(id, userId);

            if (post == null)
            {
                return NotFound();
            }

            // Only allow updates to drafts or scheduled posts
            if (post.Status != PostStatus.Draft && post.Status != PostStatus.Scheduled)
            {
                return BadRequest("Cannot update posts that are already published or canceled");
            }

            post.Title = updatePostDto.Title;
            post.Description = updatePostDto.Description;
            post.EventDateTime = updatePostDto.EventDateTime;
            post.CustomFontFamily = updatePostDto.CustomFontFamily;
            post.CustomPrimaryColor = updatePostDto.CustomPrimaryColor;
            post.ShowDays = updatePostDto.ShowDays;
            post.ShowHours = updatePostDto.ShowHours;
            post.ShowMinutes = updatePostDto.ShowMinutes;
            post.ShowSeconds = updatePostDto.ShowSeconds;
            post.BackgroundImageUrl = updatePostDto.BackgroundImageUrl;
            post.HasOverlay = updatePostDto.HasOverlay;
            post.RefreshIntervalInMinutes = updatePostDto.RefreshIntervalInMinutes;
            post.UpdatedAt = DateTime.UtcNow;

            if (updatePostDto.ScheduledFor.HasValue)
            {
                post.ScheduledFor = updatePostDto.ScheduledFor;
                post.Status = PostStatus.Scheduled;
            }
            else
            {
                post.ScheduledFor = null;
                post.Status = PostStatus.Draft;
            }

            await _postRepository.UpdateAsync(post);

            var countdownTimer = await _countdownTimerRepository.GetByPostIdAsync(post.Id);

            var postDto = new PostDto
            {
                Id = post.Id,
                FacebookPageId = post.FacebookPageId,
                FacebookPageName = post.FacebookPage.PageName,
                TemplateId = post.TemplateId,
                TemplateName = post.Template.Name,
                Title = post.Title,
                Description = post.Description,
                EventDateTime = post.EventDateTime,
                CustomFontFamily = post.CustomFontFamily,
                CustomPrimaryColor = post.CustomPrimaryColor,
                ShowDays = post.ShowDays,
                ShowHours = post.ShowHours,
                ShowMinutes = post.ShowMinutes,
                ShowSeconds = post.ShowSeconds,
                BackgroundImageUrl = post.BackgroundImageUrl,
                HasOverlay = post.HasOverlay,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                Status = post.Status,
                ScheduledFor = post.ScheduledFor,
                RefreshIntervalInMinutes = post.RefreshIntervalInMinutes,
                CountdownPublicId = countdownTimer?.PublicId
            };

            return Ok(postDto);
        }

        [HttpPost("{id}/publish")]
        public async Task<IActionResult> PublishPost(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var post = await _postRepository.GetUserPostByIdAsync(id, userId);

            if (post == null)
            {
                return NotFound();
            }

            // Only allow publishing of drafts
            if (post.Status != PostStatus.Draft)
            {
                return BadRequest("Only draft posts can be published immediately");
            }

            // In a real application, you would call the Facebook API to publish the post
            // This is a simplified example where we just update the status
            try
            {
                // Simulate Facebook API post creation
                string fakePostId = "facebook_" + Guid.NewGuid().ToString();

                await _postRepository.UpdatePostStatusAsync(post.Id, PostStatus.Published, fakePostId);

                return Ok(new PublishResultDto
                {
                    Success = true,
                    FacebookPostId = fakePostId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing post {PostId}", id);

                return StatusCode(500, new PublishResultDto
                {
                    Success = false,
                    ErrorMessage = "Failed to publish post to Facebook"
                });
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelPost(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var post = await _postRepository.GetUserPostByIdAsync(id, userId);

            if (post == null)
            {
                return NotFound();
            }

            // Only allow canceling of scheduled posts
            if (post.Status != PostStatus.Scheduled)
            {
                return BadRequest("Only scheduled posts can be canceled");
            }

            await _postRepository.UpdatePostStatusAsync(post.Id, PostStatus.Cancelled);

            return Ok(new { message = "Post canceled successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!await _postRepository.PostBelongsToUserAsync(id, userId))
            {
                return NotFound();
            }

            await _postRepository.DeleteAsync(id);

            return Ok(new { message = "Post deleted successfully" });
        }
    }
}
