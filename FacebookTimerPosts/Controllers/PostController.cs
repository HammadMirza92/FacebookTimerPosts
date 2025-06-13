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
                // Log the incoming request for debugging
                _logger.LogInformation("CreatePost called with data: {@CreatePostDto}", createPostDto);

                // Check if the model is valid
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                        .ToList();

                    _logger.LogWarning("Model validation failed: {@ValidationErrors}", errors);
                    return BadRequest(new { Message = "Validation failed", Errors = errors });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User not authenticated");
                    return Unauthorized("User not authenticated");
                }

                // CRITICAL: Validate Facebook Page ID
                if (createPostDto.FacebookPageId <= 0)
                {
                    _logger.LogError("Invalid Facebook Page ID: {FacebookPageId}", createPostDto.FacebookPageId);
                    return BadRequest(new { Message = "Valid Facebook Page ID is required", FacebookPageId = createPostDto.FacebookPageId });
                }

                // CRITICAL: Validate Template ID
                if (createPostDto.TemplateId <= 0)
                {
                    _logger.LogError("Invalid Template ID: {TemplateId}", createPostDto.TemplateId);
                    return BadRequest(new { Message = "Valid Template ID is required", TemplateId = createPostDto.TemplateId });
                }

                _logger.LogInformation("Creating post for user: {UserId}, Facebook Page: {FacebookPageId}, Template: {TemplateId}",
                    userId, createPostDto.FacebookPageId, createPostDto.TemplateId);

                // Verify Facebook page exists and belongs to user
                FacebookPage facebookPage;
                try
                {
                    facebookPage = await _facebookPageRepository.GetByIdAsync(createPostDto.FacebookPageId);
                    if (facebookPage == null)
                    {
                        _logger.LogError("Facebook page {FacebookPageId} not found", createPostDto.FacebookPageId);
                        return BadRequest(new { Message = $"Facebook page with ID {createPostDto.FacebookPageId} not found" });
                    }

                    if (facebookPage.UserId != userId)
                    {
                        _logger.LogError("Facebook page {FacebookPageId} does not belong to user {UserId}", createPostDto.FacebookPageId, userId);
                        return BadRequest(new { Message = "Facebook page does not belong to you" });
                    }

                    _logger.LogInformation("Facebook page validation passed: {PageName}", facebookPage.PageName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error validating Facebook page {FacebookPageId}", createPostDto.FacebookPageId);
                    return BadRequest(new { Message = "Error validating Facebook page", Error = ex.Message });
                }

                // Verify template exists and is accessible
                Template template;
                try
                {
                    template = await _templateRepository.GetByIdAsync(createPostDto.TemplateId);
                    if (template == null)
                    {
                        _logger.LogError("Template {TemplateId} not found", createPostDto.TemplateId);
                        return BadRequest(new { Message = $"Template with ID {createPostDto.TemplateId} not found" });
                    }

                    _logger.LogInformation("Template validation passed: {TemplateName}", template.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error validating template {TemplateId}", createPostDto.TemplateId);
                    return BadRequest(new { Message = "Error validating template", Error = ex.Message });
                }

                // Handle empty background image URL properly
                string backgroundImageUrl = string.IsNullOrWhiteSpace(createPostDto.BackgroundImageUrl) ?
                    null : createPostDto.BackgroundImageUrl.Trim();

                // Calculate NextRefreshTime if RefreshIntervalInMinutes is provided
                DateTime? nextRefreshTime = null;
                if (createPostDto.RefreshIntervalInMinutes.HasValue && createPostDto.RefreshIntervalInMinutes.Value > 0)
                {
                    nextRefreshTime = DateTime.UtcNow.AddMinutes(createPostDto.RefreshIntervalInMinutes.Value);
                }

                // Modify description to include site URL
                string originalDescription = createPostDto.Description?.Trim() ?? string.Empty;
                string siteUrl = "https://localhost:4200/facebooktimerpost";
                string descriptionWithUrl = $"{originalDescription}\n\nCheck out our countdown timer: {siteUrl}";

                // Determine if the post should be published immediately
                bool shouldPublishImmediately = !createPostDto.ScheduledFor.HasValue;

                // Create the post object with initial draft status
                var post = new Post
                {
                    UserId = userId,
                    FacebookPageId = createPostDto.FacebookPageId,
                    TemplateId = createPostDto.TemplateId,
                    Title = createPostDto.Title?.Trim() ?? string.Empty,
                    Description = descriptionWithUrl, // Use the modified description with URL
                    EventDateTime = createPostDto.EventDateTime,
                    CustomFontFamily = createPostDto.CustomFontFamily?.Trim(),
                    CustomPrimaryColor = createPostDto.CustomPrimaryColor?.Trim(),
                    ShowDays = createPostDto.ShowDays,
                    ShowHours = createPostDto.ShowHours,
                    ShowMinutes = createPostDto.ShowMinutes,
                    ShowSeconds = createPostDto.ShowSeconds,
                    BackgroundImageUrl = backgroundImageUrl,
                    HasOverlay = createPostDto.HasOverlay,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    RefreshIntervalInMinutes = createPostDto.RefreshIntervalInMinutes,
                    NextRefreshTime = nextRefreshTime,
                    FacebookPostId = null,
                    PublishedAt = null,
                    ScheduledFor = createPostDto.ScheduledFor,
                    // Set initial status based on whether it's scheduled
                    Status = createPostDto.ScheduledFor.HasValue ? PostStatus.Scheduled : PostStatus.Draft
                };

                // Set the Facebook page and template for the post object (needed for Facebook service)
                post.FacebookPage = facebookPage;
                post.Template = template;

                // STEP 1: Save to database first with appropriate status
                Post savedPost;
                try
                {
                    _logger.LogInformation("Saving post to database with initial status: {Status}", post.Status);
                    savedPost = await _postRepository.AddAsync(post);
                    _logger.LogInformation("Post {PostId} saved successfully to database", savedPost.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save post to database");
                    return BadRequest(new { Message = "Failed to save post to database", Error = ex.Message });
                }

                // STEP 2: Create countdown timer for the saved post
                CountdownTimer countdownTimer = null;
                try
                {
                    countdownTimer = await _countdownTimerRepository.CreateTimerForPostAsync(savedPost.Id);
                    _logger.LogInformation("Countdown timer created for post {PostId} with public ID {PublicId}", savedPost.Id, countdownTimer.PublicId);
                }
                catch (Exception timerEx)
                {
                    _logger.LogError(timerEx, "Error creating countdown timer for post {PostId}", savedPost.Id);
                    // Don't fail the entire operation if just the timer creation fails
                }

                // STEP 3: Publish to Facebook only if it's an immediate publish (not scheduled)
                if (shouldPublishImmediately)
                {
                    try
                    {
                        _logger.LogInformation("Attempting to publish post {PostId} to Facebook", savedPost.Id);
                        var publishResult = await _facebookService.PublishPostAsync(savedPost);

                        if (!publishResult.Success)
                        {
                            _logger.LogError("Failed to publish post to Facebook: {ErrorMessage}", publishResult.ErrorMessage);
                            // Don't delete the post, but return error
                            return BadRequest(new
                            {
                                Message = "Failed to publish post to Facebook",
                                Error = publishResult.ErrorMessage,
                                PostId = savedPost.Id
                            });
                        }

                        _logger.LogInformation("Successfully published post to Facebook with ID: {FacebookPostId}", publishResult.PostId);

                        // Update post with Facebook details
                        savedPost.FacebookPostId = publishResult.PostId;
                        savedPost.Status = PostStatus.Published;
                        savedPost.PublishedAt = DateTime.UtcNow;

                        // Update NextRefreshTime if refresh interval is set
                        if (savedPost.RefreshIntervalInMinutes.HasValue && savedPost.RefreshIntervalInMinutes.Value > 0)
                        {
                            savedPost.NextRefreshTime = DateTime.UtcNow.AddMinutes(savedPost.RefreshIntervalInMinutes.Value);
                        }

                        // Update the post in database with Facebook info
                        await _postRepository.UpdateAsync(savedPost);
                        _logger.LogInformation("Updated post {PostId} with Facebook post ID {FacebookPostId}", savedPost.Id, savedPost.FacebookPostId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Exception occurred while publishing to Facebook");
                        return BadRequest(new
                        {
                            Message = "Failed to publish post to Facebook",
                            Error = ex.Message,
                            PostId = savedPost.Id
                        });
                    }
                }

                // STEP 4: Create response DTO
                var postDto = new PostDto
                {
                    Id = savedPost.Id,
                    FacebookPageId = savedPost.FacebookPageId,
                    FacebookPageName = facebookPage.PageName,
                    TemplateId = savedPost.TemplateId,
                    TemplateName = template.Name,
                    Title = savedPost.Title,
                    Description = savedPost.Description, // Return the full description with URL
                    EventDateTime = savedPost.EventDateTime,
                    CustomFontFamily = savedPost.CustomFontFamily,
                    CustomPrimaryColor = savedPost.CustomPrimaryColor,
                    ShowDays = savedPost.ShowDays,
                    ShowHours = savedPost.ShowHours,
                    ShowMinutes = savedPost.ShowMinutes,
                    ShowSeconds = savedPost.ShowSeconds,
                    BackgroundImageUrl = savedPost.BackgroundImageUrl,
                    HasOverlay = savedPost.HasOverlay,
                    CreatedAt = savedPost.CreatedAt,
                    UpdatedAt = savedPost.UpdatedAt,
                    FacebookPostId = savedPost.FacebookPostId,
                    Status = savedPost.Status,
                    PublishedAt = savedPost.PublishedAt,
                    ScheduledFor = savedPost.ScheduledFor,
                    RefreshIntervalInMinutes = savedPost.RefreshIntervalInMinutes,
                    NextRefreshTime = savedPost.NextRefreshTime,
                    CountdownPublicId = countdownTimer?.PublicId
                };

                _logger.LogInformation("Post creation completed successfully. Status: {Status}, Facebook ID: {FacebookPostId}, Database ID: {DatabaseId}",
                    savedPost.Status, savedPost.FacebookPostId, savedPost.Id);

                return CreatedAtAction(nameof(GetPost), new { id = savedPost.Id }, postDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in CreatePost: {ErrorMessage}", ex.Message);
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while creating the post",
                    Error = ex.Message
                });
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

                // Get the post with related data
                var post = await _postRepository.GetByIdAsync(id);
                if (post == null || post.UserId != userId)
                {
                    return NotFound("Post not found");
                }

                // Verify post can be published
                if (post.Status == PostStatus.Published)
                {
                    return BadRequest(new { success = false, errorMessage = "Post is already published" });
                }

                if (post.Status == PostStatus.Cancelled)
                {
                    return BadRequest(new { success = false, errorMessage = "Cannot publish a cancelled post" });
                }

                _logger.LogInformation($"Starting to publish post {id} for user {userId}");

                // Publish to Facebook using the updated service (no need to pass pageAccessToken)
                var publishResult = await _facebookService.PublishPostAsync(post);

                if (!publishResult.Success)
                {
                    _logger.LogError($"Failed to publish post {id} to Facebook: {publishResult.ErrorMessage}");
                    return StatusCode(500, new
                    {
                        success = false,
                        errorMessage = publishResult.ErrorMessage ?? "Failed to publish post to Facebook"
                    });
                }

                // Update the post record
                post.FacebookPostId = publishResult.PostId;
                post.Status = PostStatus.Published;
                post.PublishedAt = DateTime.UtcNow;

                // If the post has a refresh interval, set the next refresh time
                if (post.RefreshIntervalInMinutes.HasValue && post.RefreshIntervalInMinutes.Value > 0)
                {
                    post.NextRefreshTime = DateTime.UtcNow.AddMinutes(post.RefreshIntervalInMinutes.Value);
                }

                await _postRepository.UpdateAsync(post);

                _logger.LogInformation($"Post {id} published successfully with Facebook post ID: {publishResult.PostId}");

                return Ok(new
                {
                    success = true,
                    facebookPostId = publishResult.PostId,
                    message = "Post published successfully to Facebook with countdown image!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unhandled error publishing post {id}");
                return StatusCode(500, new
                {
                    success = false,
                    errorMessage = "An unexpected error occurred while publishing the post. Please try again."
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
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var post = await _postRepository.GetUserPostByIdAsync(id, userId);

                if (post == null)
                {
                    return NotFound("Post not found");
                }

                _logger.LogInformation("Attempting to delete post {PostId} for user {UserId}. Status: {Status}, FacebookPostId: {FacebookPostId}",
                    id, userId, post.Status, post.FacebookPostId ?? "NULL");

                var deletionResults = new
                {
                    LocalDeletion = false,
                    FacebookDeletion = false,
                    FacebookError = (string)null,
                    PostHadFacebookId = !string.IsNullOrEmpty(post.FacebookPostId),
                    PageIsLinked = true // Default value, will update below
                };

                // Check if the Facebook page for this post is still linked
                string facebookPageId = post.FacebookPage?.PageId;
                bool pageIsLinked = false;

                if (!string.IsNullOrEmpty(facebookPageId))
                {
                    pageIsLinked = await _facebookPageRepository.IsPageLinkedAsync(facebookPageId);
                    deletionResults = deletionResults with { PageIsLinked = pageIsLinked };
                }

                // Try to delete from Facebook first if post has FacebookPostId AND page is linked
                if (!string.IsNullOrEmpty(post.FacebookPostId) && pageIsLinked)
                {
                    try
                    {
                        _logger.LogInformation("Post {PostId} has Facebook Post ID {FacebookPostId} and page is linked, attempting Facebook deletion",
                            id, post.FacebookPostId);

                        var page = await _facebookPageRepository.GetByIdAsync(post.FacebookPageId);
                        if (page == null)
                        {
                            _logger.LogWarning("Facebook page {FacebookPageId} not found for post {PostId}", post.FacebookPageId, id);
                            deletionResults = deletionResults with { FacebookError = "Facebook page not found" };
                        }
                        else if (string.IsNullOrEmpty(page.PageAccessToken))
                        {
                            _logger.LogWarning("Page access token is missing for page {FacebookPageId}", post.FacebookPageId);
                            deletionResults = deletionResults with { FacebookError = "Page access token is missing" };
                        }
                        else
                        {
                            _logger.LogInformation("Found Facebook page {PageName} (ID: {PageId}) for post {PostId}",
                                page.PageName, page.Id, id);

                            // Validate access token first
                            var tokenValid = await _facebookService.ValidateAccessTokenAsync(page.PageAccessToken);
                            if (!tokenValid)
                            {
                                _logger.LogError("Invalid or expired access token for page {PageId}", page.Id);
                                deletionResults = deletionResults with { FacebookError = "Invalid or expired access token" };
                            }
                            else
                            {
                                // Check if post exists on Facebook
                                var postExists = await _facebookService.PostExistsAsync(post.FacebookPostId, page.PageAccessToken);
                                if (!postExists)
                                {
                                    _logger.LogWarning("Post {FacebookPostId} does not exist on Facebook (may have been deleted already)", post.FacebookPostId);
                                    deletionResults = deletionResults with { FacebookDeletion = true }; // Consider it successful since post is already gone
                                }
                                else
                                {
                                    // Attempt to delete from Facebook
                                    var facebookDeleted = await _facebookService.DeletePostAsync(post.FacebookPostId, page.PageAccessToken);
                                    deletionResults = deletionResults with { FacebookDeletion = facebookDeleted };

                                    if (facebookDeleted)
                                    {
                                        _logger.LogInformation("Successfully deleted post {FacebookPostId} from Facebook", post.FacebookPostId);
                                    }
                                    else
                                    {
                                        _logger.LogError("Failed to delete post {FacebookPostId} from Facebook", post.FacebookPostId);
                                        deletionResults = deletionResults with { FacebookError = "Facebook API deletion failed" };
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Exception during Facebook deletion for post {PostId}: {Message}", id, ex.Message);
                        deletionResults = deletionResults with { FacebookError = ex.Message };
                    }
                }
                else if (!string.IsNullOrEmpty(post.FacebookPostId) && !pageIsLinked)
                {
                    _logger.LogInformation("Post {PostId} has Facebook Post ID {FacebookPostId} but page is not linked", id, post.FacebookPostId);
                }
                else
                {
                    _logger.LogInformation("Post {PostId} has no Facebook Post ID, skipping Facebook deletion", id);
                }

                // Delete from local database
                try
                {
                    await _postRepository.DeleteAsync(id);
                    deletionResults = deletionResults with { LocalDeletion = true };
                    _logger.LogInformation("Successfully deleted post {PostId} from local database", id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete post {PostId} from local database: {Message}", id, ex.Message);
                    return StatusCode(500, new
                    {
                        message = "Failed to delete post from database",
                        error = ex.Message,
                        deletionResults
                    });
                }

                // Prepare response based on deletion results
                if (!deletionResults.PostHadFacebookId)
                {
                    return Ok(new
                    {
                        message = "Post deleted successfully from database",
                        deletionResults
                    });
                }
                else if (!deletionResults.PageIsLinked)
                {
                    return Ok(new
                    {
                        message = "Post deleted from database only",
                        warning = "Page is not linked. We are just deleting this post in our database not from Facebook. If you want to delete the post from Facebook please link same page again.",
                        deletionResults
                    });
                }
                else if (deletionResults.FacebookDeletion)
                {
                    return Ok(new
                    {
                        message = "Post deleted successfully from both Facebook and database",
                        deletionResults
                    });
                }
                else
                {
                    return Ok(new
                    {
                        message = "Post deleted from database, but Facebook deletion failed",
                        warning = "The post may still exist on Facebook",
                        deletionResults
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error deleting post {PostId}: {Message}", id, ex.Message);
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while deleting the post",
                    error = ex.Message
                });
            }
        }
    }
}