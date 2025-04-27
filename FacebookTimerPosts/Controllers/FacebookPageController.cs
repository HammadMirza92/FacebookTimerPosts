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
    //[Authorize]
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

        [HttpGet("get-pages")]
        public async Task<IActionResult> GetUserPagesAsync(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                return BadRequest("Access token is required");
            }

            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_graphApiVersion}/me/accounts?access_token={accessToken}");

                var rawJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, $"Facebook API error: {rawJson}");
                }

                // Parse as dynamic to see the structure
                var dynamicContent = JsonSerializer.Deserialize<JsonElement>(rawJson);

                // Return the raw structure to inspect it
                return Ok(dynamicContent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<string> CreatePostAsync(string pageId, string pageAccessToken, PostContent postContent)
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
            return responseContent?.Id;
        }

        // Helper classes for serialization/deserialization
        public class FacebookPageResponse
        {
            public List<LinkPageDto> Data { get; set; }
        }

        public class FacebookPageDto
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public string Access_Token { get; set; }
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
