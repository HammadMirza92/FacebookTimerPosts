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
    public class FacebookPagesController : ControllerBase
    {
        private readonly IFacebookPageRepository _pageRepository;
        private readonly IMapper _mapper;

        public FacebookPagesController(
            IFacebookPageRepository pageRepository,
            IMapper mapper)
        {
            _pageRepository = pageRepository;
            _mapper = mapper;
        }

        [HttpPost("link")]
        public async Task<ActionResult<FacebookPageDto>> LinkFacebookPage(LinkPageDto linkPageDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Verify the token and get page details
            var pageDetails = await _pageRepository.GetPageDetailsAsync(
                linkPageDto.PageId,
                linkPageDto.AccessToken);

            if (pageDetails == null) return BadRequest("Invalid page ID or access token");

            // Check if page is already linked
            var existingPage = await _pageRepository.GetPageByFacebookIdAsync(pageDetails.Id);
            if (existingPage != null)
                return BadRequest("This page is already linked to an account");

            // Create and link the page
            var page = new FacebookPage
            {
                FacebookPageId = pageDetails.Id,
                PageName = pageDetails.Name,
                AccessToken = linkPageDto.AccessToken,
                ProfilePictureUrl = pageDetails.PictureUrl
            };

            var success = await _pageRepository.LinkPageToUserAsync(page, userId);

            if (!success) return BadRequest("Failed to link page");

            return _mapper.Map<FacebookPageDto>(page);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> UnlinkFacebookPage(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var success = await _pageRepository.UnlinkPageFromUserAsync(id, userId);

            if (!success) return NotFound();

            return NoContent();
        }

        [HttpGet("{id}/posts")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPagePosts(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var page = await _pageRepository.GetPageWithPostsAsync(id);

            if (page == null || page.UserId != userId) return NotFound();

            return Ok(_mapper.Map<IEnumerable<PostDto>>(page.Posts));
        }
    }
}
