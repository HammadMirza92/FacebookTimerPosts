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
    public class FacebookPageController : ControllerBase
    {
        private readonly IFacebookPageRepository _facebookPageRepository;

        public FacebookPageController(IFacebookPageRepository facebookPageRepository)
        {
            _facebookPageRepository = facebookPageRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPages()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var pages = await _facebookPageRepository.GetUserPagesAsync(userId);

            var pagesDto = new List<FacebookPageDetailsDto>();
            foreach (var page in pages)
            {
                pagesDto.Add(new FacebookPageDetailsDto
                {
                    Id = page.Id,
                    PageId = page.PageId,
                    PageName = page.PageName,
                    TokenExpiryDate = page.TokenExpiryDate,
                    IsActive = page.IsActive,
                    CreatedAt = page.CreatedAt,
                    UpdatedAt = page.UpdatedAt
                });
            }

            return Ok(pagesDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPage(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var page = await _facebookPageRepository.GetUserPageByIdAsync(id, userId);

            if (page == null)
            {
                return NotFound();
            }

            var pageDto = new FacebookPageDetailsDto
            {
                Id = page.Id,
                PageId = page.PageId,
                PageName = page.PageName,
                TokenExpiryDate = page.TokenExpiryDate,
                IsActive = page.IsActive,
                CreatedAt = page.CreatedAt,
                UpdatedAt = page.UpdatedAt
            };

            return Ok(pageDto);
        }

        [HttpPost("link")]
        public async Task<IActionResult> LinkFacebookPage([FromBody] LinkPageDto linkPageDto)
        {
            if (string.IsNullOrEmpty(linkPageDto.AccessToken) ||
                string.IsNullOrEmpty(linkPageDto.PageId) ||
                string.IsNullOrEmpty(linkPageDto.PageName))
            {
                return BadRequest("Missing required information");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // In a real-world scenario, you'd validate the token with Facebook
            // and get more accurate expiry date
            var expiryDate = DateTime.UtcNow.AddDays(60);

            var page = await _facebookPageRepository.LinkFacebookPageAsync(
                userId, linkPageDto.PageId, linkPageDto.PageName,
                linkPageDto.AccessToken, expiryDate);

            var pageDto = new FacebookPageDetailsDto
            {
                Id = page.Id,
                PageId = page.PageId,
                PageName = page.PageName,
                TokenExpiryDate = page.TokenExpiryDate,
                IsActive = page.IsActive,
                CreatedAt = page.CreatedAt,
                UpdatedAt = page.UpdatedAt
            };

            return Ok(pageDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> UnlinkPage(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var page = await _facebookPageRepository.GetUserPageByIdAsync(id, userId);

            if (page == null)
            {
                return NotFound();
            }

            page.IsActive = false;
            page.UpdatedAt = DateTime.UtcNow;

            await _facebookPageRepository.UpdateAsync(page);

            return Ok(new { message = "Page unlinked successfully" });
        }
    }
}
