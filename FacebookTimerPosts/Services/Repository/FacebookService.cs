using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FacebookTimerPosts.Services.Repository
{
    public class FacebookService : IFacebookService
    {
        private readonly IFacebookPageRepository _facebookPageRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FacebookService> _logger;

        public FacebookService(
            IFacebookPageRepository facebookPageRepository,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<FacebookService> logger)
        {
            _facebookPageRepository = facebookPageRepository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<(bool Success, string PostId, string ErrorMessage)> PublishPostAsync(Post post, string imageUrl)
        {
            try
            {
                // Get the Facebook page
                var page = await _facebookPageRepository.GetByIdAsync((int)post.FacebookPageId);
                if (page == null)
                {
                    return (false, null, "Facebook page not found");
                }

                // Validate token first
                var tokenResult = await ValidatePageAccessTokenAsync(page);
                if (!tokenResult.Success)
                {
                    return (false, null, tokenResult.ErrorMessage);
                }

                string pageAccessToken = tokenResult.PageAccessToken;

                // Create Facebook client
                var client = _httpClientFactory.CreateClient("FacebookGraph");

                // Build the post content
                string postContent = $"{post.Title}\n\n{post.Description}\n\nEvent Date: {post.EventDateTime:g}";

                // First upload the image if provided
                string attachmentId = null;
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    var imageUploadResult = await UploadImageAsync(page.PageId, imageUrl, pageAccessToken);
                    if (imageUploadResult.Success)
                    {
                        attachmentId = imageUploadResult.AttachmentId;
                    }
                    else
                    {
                        _logger.LogWarning("Failed to upload image: {ErrorMessage}", imageUploadResult.ErrorMessage);
                        // Continue without image if upload fails
                    }
                }

                // Create the post payload
                var postData = new Dictionary<string, string>
                {
                    { "message", postContent },
                    { "access_token", pageAccessToken }
                };

                // Add image attachment if available
                if (!string.IsNullOrEmpty(attachmentId))
                {
                    postData.Add("attached_media[0]", $"{{\"media_fbid\":\"{attachmentId}\"}}");
                }

                // Post to Facebook
                var content = new FormUrlEncodedContent(postData);

                // Use the page ID from the database, not the post.FacebookPageId which is the DB relation ID
                var response = await client.PostAsync($"{page.PageId}/feed", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Facebook API error: {StatusCode} - {Response}", response.StatusCode, errorContent);
                    return (false, null, $"Failed to create post: {response.StatusCode} - {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent);

                if (responseData.TryGetValue("id", out var idElement))
                {
                    string postId = idElement.GetString();
                    return (true, postId, null);
                }

                return (false, null, "Post created but could not retrieve post ID");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Facebook post");
                return (false, null, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool Success, string PageAccessToken, string ErrorMessage)> ValidatePageAccessTokenAsync(FacebookPage page)
        {
            try
            {
                // Check if token is expired
                if (page.TokenExpiryDate <= DateTime.UtcNow)
                {
                    // Try to refresh the token
                    // This would normally use the app token to refresh user tokens
                    // But we'll just return an error for now
                    return (false, null, "Page access token has expired. Please re-authenticate.");
                }

                // Create Facebook client
                var client = _httpClientFactory.CreateClient("FacebookGraph");

                // Validate the token
                var response = await client.GetAsync($"debug_token?input_token={page.PageAccessToken}&access_token={_configuration["Facebook:AppId"]}|{_configuration["Facebook:AppSecret"]}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Token validation error: {StatusCode} - {Response}", response.StatusCode, errorContent);
                    return (false, null, "Invalid page access token");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<JsonElement>(responseContent);

                // Check if the token is valid
                if (responseData.TryGetProperty("data", out var data) &&
                    data.TryGetProperty("is_valid", out var isValid) &&
                    isValid.GetBoolean())
                {
                    return (true, page.PageAccessToken, null);
                }

                return (false, null, "Invalid page access token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating page access token");
                return (false, null, $"Error: {ex.Message}");
            }
        }

        private async Task<(bool Success, string AttachmentId, string ErrorMessage)> UploadImageAsync(string pageId, string imageUrl, string accessToken)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("FacebookGraph");

                // Download the image first if it's a remote URL
                byte[] imageBytes;
                if (imageUrl.StartsWith("http"))
                {
                    using var httpClient = new HttpClient();
                    imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                }
                else
                {
                    // Assume local file path
                    imageBytes = await System.IO.File.ReadAllBytesAsync(imageUrl);
                }

                // Create multipart form data content
                using var content = new MultipartFormDataContent();
                using var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg"); // Adjust if needed

                content.Add(imageContent, "source", "image.jpg");
                content.Add(new StringContent(accessToken), "access_token");

                // Upload the image to Facebook
                var response = await client.PostAsync($"{pageId}/photos?published=false", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Image upload error: {StatusCode} - {Response}", response.StatusCode, errorContent);
                    return (false, null, $"Failed to upload image: {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent);

                if (responseData.TryGetValue("id", out var idElement))
                {
                    string attachmentId = idElement.GetString();
                    return (true, attachmentId, null);
                }

                return (false, null, "Failed to get attachment ID");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image to Facebook");
                return (false, null, $"Error: {ex.Message}");
            }
        }

        public async Task<bool> DeletePostAsync(string postId, string pageAccessToken)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("FacebookGraph");

                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "access_token", pageAccessToken }
                });

                var response = await client.SendAsync(new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri($"https://graph.facebook.com/v22.0/{postId}"),
                    Content = content
                });

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Delete post error: {StatusCode} - {Response}", response.StatusCode, errorContent);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Facebook post");
                return false;
            }
        }
    }
}
