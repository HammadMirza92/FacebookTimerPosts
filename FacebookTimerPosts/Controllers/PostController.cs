using FacebookTimerPosts.DTOs;
using FacebookTimerPosts.Enums;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services;
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
        private readonly IFacebookService _facebookService;
        private readonly ILogger<PostController> _logger;

        public PostController(
            IPostRepository postRepository,
            IFacebookPageRepository facebookPageRepository,
            ITemplateRepository templateRepository,
            ICountdownTimerRepository countdownTimerRepository,
            IUserSubscriptionRepository userSubscriptionRepository,
            IFacebookService facebookService,
            ILogger<PostController> logger)
        {
            _postRepository = postRepository;
            _facebookPageRepository = facebookPageRepository;
            _templateRepository = templateRepository;
            _countdownTimerRepository = countdownTimerRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _facebookService = facebookService;
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
                    NextRefreshTime = post.NextRefreshTime,
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
            //if (!await _facebookPageRepository.PageBelongsToUserAsync(pageId, userId))
            //{
            //    return NotFound();
            //}

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
                    NextRefreshTime = post.NextRefreshTime,
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
                NextRefreshTime = post.NextRefreshTime,
                CountdownPublicId = countdownTimer?.PublicId
            };

            return Ok(postDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto createPostDto)
        {
            try
            {
                // Check if the model is valid
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                // Verify page belongs to user - with error handling
                try
                {
                    //if (!await _facebookPageRepository.PageBelongsToUserAsync(createPostDto.FacebookPageId, userId))
                    //{
                    //    return BadRequest("Facebook page not found or doesn't belong to you");
                    //}
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking page ownership");
                    return StatusCode(500, "Error validating Facebook page ownership");
                }

                // Verify template - with error handling
                try
                {
                    var userSubscription = await _userSubscriptionRepository.GetCurrentSubscriptionAsync(userId);
                    int? subscriptionPlanId = userSubscription?.SubscriptionPlanId;
                    if (!await _templateRepository.IsTemplateAccessibleToUserAsync(createPostDto.TemplateId, userId, subscriptionPlanId))
                    {
                        return BadRequest("Template not found or not accessible with your subscription");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking template access");
                    return StatusCode(500, "Error validating template access");
                }

                // Handle empty background image URL properly
                string backgroundImageUrl = string.IsNullOrWhiteSpace(createPostDto.BackgroundImageUrl) ?
                    null : createPostDto.BackgroundImageUrl;

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
                    BackgroundImageUrl = backgroundImageUrl,
                    HasOverlay = createPostDto.HasOverlay,
                    CreatedAt = DateTime.UtcNow,
                    Status = PostStatus.Draft,
                    RefreshIntervalInMinutes = createPostDto.RefreshIntervalInMinutes,
                    NextRefreshTime = createPostDto.NextRefreshTime
                };

                if (createPostDto.ScheduledFor != null)
                {
                    post.ScheduledFor = createPostDto.ScheduledFor;
                    post.Status = PostStatus.Scheduled;
                }

                try
                {
                    await _postRepository.AddAsync(post);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving post to database");
                    return StatusCode(500, "Failed to save post to database");
                }

                // Create countdown timer with error handling
                try
                {
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
                        NextRefreshTime = post.NextRefreshTime,
                        CountdownPublicId = countdownTimer.PublicId
                    };

                    return CreatedAtAction(nameof(GetPost), new { id = post.Id }, postDto);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error creating countdown timer for post {post.Id}");
                    // Since the post was created but the countdown timer failed, we should clean up
                    try
                    {
                        await _postRepository.DeleteAsync(post.Id);
                    }
                    catch
                    {
                        // Log but continue with the error response
                        _logger.LogWarning($"Failed to clean up post {post.Id} after countdown timer creation failed");
                    }
                    return StatusCode(500, "Failed to create countdown timer for post");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in CreatePost");
                return StatusCode(500, "An unexpected error occurred while creating the post");
            }
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
            post.NextRefreshTime = updatePostDto.NextRefreshTime;
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
                NextRefreshTime = post.NextRefreshTime,
                CountdownPublicId = countdownTimer?.PublicId
            };

            return Ok(postDto);
        }

        [HttpPost("{id}/publish")]
        public async Task<IActionResult> PublishPost(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Get the post
                var post = await _postRepository.GetByIdAsync(id);
                if (post == null || post.UserId != userId)
                {
                    return NotFound("Post not found");
                }

                // Get page access token
                var pageAccessToken = await _facebookPageRepository.GetPageAccessTokenAsync(post.FacebookPageId);
                if (string.IsNullOrEmpty(pageAccessToken))
                {
                    return BadRequest("Facebook page access token not found");
                }

                // Publish to Facebook
                var facebookPostId = await _facebookService.PublishPostAsync(post, pageAccessToken);

                // Update the post record
                post.FacebookPostId = facebookPostId.PostId;
                post.Status = PostStatus.Published;
                post.PublishedAt = DateTime.UtcNow;

                // If the post has a refresh interval, set the next refresh time
                if (post.RefreshIntervalInMinutes.HasValue && post.RefreshIntervalInMinutes.Value > 0)
                {
                    post.NextRefreshTime = DateTime.UtcNow.AddMinutes(post.RefreshIntervalInMinutes.Value);
                }

                await _postRepository.UpdateAsync(post);

                return Ok(new { success = true, facebookPostId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing post");
                return StatusCode(500, new { success = false, errorMessage = ex.Message });
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
            var post = await _postRepository.GetUserPostByIdAsync(id, userId);

            if (post == null)
            {
                return NotFound();
            }

            // If post is published on Facebook, try to delete it there too
            if (post.Status == PostStatus.Published && !string.IsNullOrEmpty(post.FacebookPostId))
            {
                try
                {
                    var page = await _facebookPageRepository.GetByIdAsync(post.FacebookPageId);
                    if (page != null)
                    {
                        await _facebookService.DeletePostAsync(post.FacebookPostId, page.PageAccessToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete post from Facebook, continuing with local deletion");
                    // Continue with local deletion even if Facebook deletion fails
                }
            }

            await _postRepository.DeleteAsync(id);

            return Ok(new { message = "Post deleted successfully" });
        }
    }
}