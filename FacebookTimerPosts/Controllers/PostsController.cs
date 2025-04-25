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
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFacebookPageRepository _pageRepository;
        private readonly IMapper _mapper;

        public PostsController(
            IPostRepository postRepository,
            IUserRepository userRepository,
            IFacebookPageRepository pageRepository,
            IMapper mapper)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _pageRepository = pageRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetUserPosts()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var posts = await _postRepository.GetUserPostsAsync(userId);

            return Ok(_mapper.Map<IEnumerable<PostDto>>(posts));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPost(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var post = await _postRepository.GetPostWithDetailsAsync(id);

            if (post == null || post.UserId != userId) return NotFound();

            return _mapper.Map<PostDto>(post);
        }

        [HttpPost]
        public async Task<ActionResult<PostDto>> CreatePost(CreatePostDto createPostDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Check if user has reached daily post limit
            var remainingPosts = await _userRepository.GetUserRemainingPostsForTodayAsync(userId);

            if (remainingPosts <= 0)
                return BadRequest("You have reached your daily post limit");

            // Create the post
            var post = _mapper.Map<Post>(createPostDto);
            post.UserId = userId;

            // Generate a unique URL for the countdown
            post.CountdownUrl = $"/countdown/{Guid.NewGuid().ToString()}";

            await _postRepository.AddAsync(post);
            var success = await _postRepository.SaveAllAsync();

            if (!success) return BadRequest("Failed to create post");

            // Load related data for the response
            post = await _postRepository.GetPostWithDetailsAsync(post.Id);

            return CreatedAtAction(
                nameof(GetPost),
                new { id = post.Id },
                _mapper.Map<PostDto>(post));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePost(int id, UpdatePostDto updatePostDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var post = await _postRepository.GetByIdAsync(id);

            if (post == null || post.UserId != userId) return NotFound();

            // Only allow updates to drafts or if the target date hasn't passed
            if (post.Status != PostStatus.Draft && post.TargetDate <= DateTime.UtcNow)
                return BadRequest("Cannot update an expired post");

            _mapper.Map(updatePostDto, post);
            post.LastUpdated = DateTime.UtcNow;

            _postRepository.Update(post);
            var success = await _postRepository.SaveAllAsync();

            if (!success) return BadRequest("Failed to update post");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePost(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var post = await _postRepository.GetByIdAsync(id);

            if (post == null || post.UserId != userId) return NotFound();

            _postRepository.Delete(post);
            var success = await _postRepository.SaveAllAsync();

            if (!success) return BadRequest("Failed to delete post");

            return NoContent();
        }

        [HttpPost("{id}/publish")]
        public async Task<ActionResult> PublishPost(int id, PublishPostDto publishDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var post = await _postRepository.GetPostWithDetailsAsync(id);

            if (post == null || post.UserId != userId) return NotFound();
            if (post.Status == PostStatus.Published) return BadRequest("Post is already published");

            // Post to Facebook
            var fbPostId = await _pageRepository.CreatePostAsync(
                post.FacebookPage.FacebookPageId,
                post.FacebookPage.AccessToken,
                $"{post.Description}\n\nCheck out our countdown: {publishDto.BaseUrl}{post.CountdownUrl}");

            if (string.IsNullOrEmpty(fbPostId))
                return BadRequest("Failed to post to Facebook");

            // Update post status
            post.Status = PostStatus.Published;
            post.LastUpdated = DateTime.UtcNow;

            _postRepository.Update(post);
            var success = await _postRepository.SaveAllAsync();

            if (!success) return BadRequest("Failed to update post status");

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("countdown/{urlSegment}")]
        public async Task<ActionResult<CountdownViewDto>> GetCountdown(string urlSegment)
        {
            var post = await _postRepository.GetPostByCountdownUrlAsync(urlSegment);

            if (post == null || post.Status != PostStatus.Published)
                return NotFound();

            if (post.TargetDate <= DateTime.UtcNow)
            {
                post.Status = PostStatus.Expired;
                _postRepository.Update(post);
                await _postRepository.SaveAllAsync();
                return NotFound("Countdown has expired");
            }

            return _mapper.Map<CountdownViewDto>(post);
        }
    }
}
