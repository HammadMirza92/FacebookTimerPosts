using FacebookTimerPosts.DTOs;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;

namespace FacebookTimerPosts.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FacebookPageController : ControllerBase
    {
        private readonly IFacebookPageRepository _facebookPageRepository;
        private readonly HttpClient _httpClient;
        private readonly string _graphApiVersion = "v18.0";
        private readonly string _appId;
        private readonly string _appSecret;
        public FacebookPageController(IFacebookPageRepository facebookPageRepository, HttpClient httpClient, IConfiguration configuration)
        {
            _facebookPageRepository = facebookPageRepository;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://graph.facebook.com/");

            _appId = configuration["Facebook:AppId"];
            _appSecret = configuration["Facebook:AppSecret"];
        }

        [HttpGet]
        [Route("get-user-pages")]
        public async Task<IActionResult> GetUserPages()
        {
            try
            {
                // Get the current user's ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User must be authenticated");
                }

                // Get the user's Facebook pages from the database
                var pages = await _facebookPageRepository.GetUserPagesAsync(userId);

                return Ok(pages.Select(p => new {
                    p.Id,
                    p.PageId,
                    p.PageName,
                    p.IsActive,
                    p.CreatedAt
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving Facebook pages: {ex.Message}");
            }
        }

        [HttpGet("link-pages")]
        public async Task<IActionResult> LinkFacebookPages(string accessTokenFb)
        {
            if (string.IsNullOrEmpty(accessTokenFb))
            {
                return BadRequest("Access token is required");
            }
            try
            {
                // Get the current user's ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User must be authenticated");
                }
                var response = await _httpClient.GetAsync(
                    $"{_graphApiVersion}/me/accounts?access_token={accessTokenFb}");
                var rawJson = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, $"Facebook API error: {rawJson}");
                }
                // Deserialize into JsonElement to handle dynamic JSON
                var pageResponse = JsonSerializer.Deserialize<JsonElement>(rawJson);
                if (pageResponse.ValueKind == JsonValueKind.Undefined ||
                    !pageResponse.TryGetProperty("data", out JsonElement dataElement) ||
                    dataElement.GetArrayLength() == 0)
                {
                    return Ok(new { Message = "No Facebook pages found" });
                }

                // Lists to track new and existing pages
                var linkedPages = new List<FacebookPage>();
                int existingPagesCount = 0;

                foreach (JsonElement page in dataElement.EnumerateArray())
                {
                    // Extract values from the JsonElement
                    string pageId = string.Empty;
                    string pageName = string.Empty;
                    string accessToken = string.Empty;
                    if (page.TryGetProperty("id", out JsonElement idElement))
                        pageId = idElement.GetString() ?? string.Empty;
                    if (page.TryGetProperty("name", out JsonElement nameElement))
                        pageName = nameElement.GetString() ?? string.Empty;
                    if (page.TryGetProperty("access_token", out JsonElement tokenElement))
                        accessToken = tokenElement.GetString() ?? string.Empty;

                    // Only proceed if we have the required data
                    if (!string.IsNullOrEmpty(pageId) && !string.IsNullOrEmpty(accessToken))
                    {
                        // Check if page already exists - now returns boolean
                        bool pageExists = await _facebookPageRepository.PageBelongsToUserAsync(pageId, userId);

                        if (pageExists)
                        {
                            // Increment counter for existing pages
                            existingPagesCount++;
                        }
                        else
                        {
                            // Page doesn't exist, create a new link
                            // Set token expiry date (typically long-lived tokens last 60 days)
                            var expiryDate = DateTime.UtcNow.AddDays(60);

                            var linkedPage = await _facebookPageRepository.LinkFacebookPageAsync(
                                userId,
                                pageId,
                                pageName,
                                accessToken,
                                expiryDate
                            );

                            linkedPages.Add(linkedPage);
                        }
                    }
                }

                // After processing, get all pages for this user to include in response
                var allUserPages = await _facebookPageRepository.GetUserPagesAsync(userId);

                // Prepare the response message based on what happened
                string message;
                if (linkedPages.Count > 0 && existingPagesCount > 0)
                {
                    message = $"{linkedPages.Count} new page(s) linked and {existingPagesCount} page(s) already exist";
                }
                else if (linkedPages.Count > 0)
                {
                    message = $"{linkedPages.Count} page(s) successfully linked to your account";
                }
                else if (existingPagesCount > 0)
                {
                    message = "All pages are already linked to your account";
                }
                else
                {
                    message = "No pages were linked";
                }

                // Return the appropriate response
                return Ok(new
                {
                    Message = message,
                    NewPagesCount = linkedPages.Count,
                    ExistingPagesCount = existingPagesCount,
                    Pages = allUserPages.Select(p => new {
                        p.Id,
                        p.PageId,
                        p.PageName,
                        p.IsActive,
                        p.CreatedAt
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePostAsync(string pageId, string pageAccessToken, PostContent postContent)
        {
            try
            {
                var requestData = new Dictionary<string, string>
                {
                    ["message"] = postContent.Message
                };

                // Add optional parameters if they exist
                if (!string.IsNullOrEmpty(postContent.Link))
                {
                    requestData.Add("link", postContent.Link);
                }

                if (!string.IsNullOrEmpty(postContent.ImageUrl))
                {
                    // For image posts, a different endpoint might be required
                    // This is a simplified approach
                    requestData.Add("url", postContent.ImageUrl);
                }

                // Add access token
                requestData.Add("access_token", pageAccessToken);

                var content = new FormUrlEncodedContent(requestData);
                var response = await _httpClient.PostAsync($"{_graphApiVersion}/{pageId}/feed", content);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadFromJsonAsync<CreatePostResponse>();
                return Ok(new { PostId = responseContent?.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating post: {ex.Message}");
            }
        }

        // Helper classes for serialization/deserialization
        public class FacebookPageResponse
        {
            public List<FacebookPageDto> Data { get; set; }
            public Paging Paging { get; set; }
        }

        public class Paging
        {
            public Cursors Cursors { get; set; }
        }

        public class Cursors
        {
            public string Before { get; set; }
            public string After { get; set; }
        }

        public class FacebookPageDto
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public string Access_Token { get; set; }
            public List<CategoryInfo> Category_List { get; set; }
            public List<string> Tasks { get; set; }
        }

        public class CategoryInfo
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class CreatePostResponse
        {
            public string Id { get; set; }
        }

        public class PostContent
        {
            public string Message { get; set; }
            public string Link { get; set; }
            public string ImageUrl { get; set; }
        }
    }
}