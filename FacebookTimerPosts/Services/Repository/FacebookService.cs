using FacebookTimerPosts.DTOs;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FacebookTimerPosts.Services.Repository
{
    public class FacebookService : IFacebookService
    {
        private readonly IFacebookPageRepository _facebookPageRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IImageGenerationService _imageGenerationService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FacebookService> _logger;
        private const string GRAPH_API_VERSION = "v18.0";

        public FacebookService(
            IFacebookPageRepository facebookPageRepository,
            ITemplateRepository templateRepository,
            IImageGenerationService imageGenerationService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<FacebookService> logger)
        {
            _facebookPageRepository = facebookPageRepository;
            _templateRepository = templateRepository;
            _imageGenerationService = imageGenerationService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<FacebookPostResult> PublishPostAsync(Post post, string imageUrl = null)
        {
            try
            {
                // Get the Facebook page
                var page = await _facebookPageRepository.GetByIdAsync(post.FacebookPageId);
                if (page == null)
                {
                    return FacebookPostResult.CreateFailure("Facebook page not found");
                }

                // Validate token first
                var tokenResult = await ValidatePageAccessTokenAsync(page);
                if (!tokenResult.Success)
                {
                    return FacebookPostResult.CreateFailure(tokenResult.ErrorMessage);
                }

                string pageAccessToken = tokenResult.PageAccessToken;

                // Get template for image generation
                var template = await _templateRepository.GetByIdAsync(post.TemplateId);
                if (template == null)
                {
                    return FacebookPostResult.CreateFailure("Template not found");
                }

                _logger.LogInformation("Starting to publish post {PostId} to Facebook page {PageId}",
                    post.Id, page.PageId);

                // Generate countdown image
                byte[] imageBytes;
                try
                {
                    imageBytes = await _imageGenerationService.GenerateCountdownImageBytesAsync(post, template);
                    _logger.LogInformation("Generated countdown image for post {PostId}, size: {Size} bytes",
                        post.Id, imageBytes.Length);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to generate countdown image for post {PostId}", post.Id);
                    return FacebookPostResult.CreateFailure($"Failed to generate countdown image: {ex.Message}");
                }

                // Create Facebook client
                var client = _httpClientFactory.CreateClient("FacebookGraph");

                // Upload image to Facebook and get attachment ID
                string attachmentId;
                try
                {
                    var imageUploadResult = await UploadImageBytesAsync(page.PageId, imageBytes, pageAccessToken);
                    if (!imageUploadResult.Success)
                    {
                        return FacebookPostResult.CreateFailure($"Failed to upload image: {imageUploadResult.ErrorMessage}");
                    }
                    attachmentId = imageUploadResult.AttachmentId;
                    _logger.LogInformation("Uploaded image to Facebook for post {PostId}, attachment ID: {AttachmentId}",
                        post.Id, attachmentId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to upload image for post {PostId}", post.Id);
                    return FacebookPostResult.CreateFailure($"Failed to upload image: {ex.Message}");
                }

                // Build the post content
                string postContent = $"{post.Title}\n\n{post.Description}\n\nEvent Date: {post.EventDateTime:g}";

                // Create the post payload
                var postData = new Dictionary<string, string>
                {
                    { "message", postContent },
                    { "access_token", pageAccessToken }
                };

                // Add image attachment
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
                    return FacebookPostResult.CreateFailure($"Failed to create post: {response.StatusCode} - {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent);

                if (responseData.TryGetValue("id", out var idElement))
                {
                    string postId = idElement.GetString();
                    _logger.LogInformation("Successfully published post {PostId} to Facebook with ID: {FacebookPostId}",
                        post.Id, postId);
                    return FacebookPostResult.CreateSuccess(postId);
                }

                return FacebookPostResult.CreateFailure("Post created but could not retrieve post ID");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Facebook post for post {PostId}", post.Id);
                return FacebookPostResult.CreateFailure($"Error: {ex.Message}");
            }
        }

        public async Task<(bool Success, string PageAccessToken, string ErrorMessage)> ValidatePageAccessTokenAsync(FacebookPage page)
        {
            try
            {
                // Check if token is expired
                if (page.TokenExpiryDate <= DateTime.UtcNow)
                {
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

        private async Task<(bool Success, string AttachmentId, string ErrorMessage)> UploadImageBytesAsync(string pageId, byte[] imageBytes, string accessToken)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("FacebookGraph");

                // Create multipart form data content
                using var content = new MultipartFormDataContent();
                using var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");

                content.Add(imageContent, "source", "countdown.png");
                content.Add(new StringContent(accessToken), "access_token");
                content.Add(new StringContent("false"), "published"); // Upload but don't publish yet

                // Upload the image to Facebook
                var response = await client.PostAsync($"{pageId}/photos", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Image upload error: {StatusCode} - {Response}", response.StatusCode, errorContent);
                    return (false, null, $"Failed to upload image: {response.StatusCode} - {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent);

                if (responseData.TryGetValue("id", out var idElement))
                {
                    string attachmentId = idElement.GetString();
                    return (true, attachmentId, null);
                }

                return (false, null, "Failed to get attachment ID from Facebook response");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image to Facebook");
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
                    imageBytes = await File.ReadAllBytesAsync(imageUrl);
                }

                return await UploadImageBytesAsync(pageId, imageBytes, accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image to Facebook");
                return (false, null, $"Error: {ex.Message}");
            }
        }
        public async Task<bool> ValidateAccessTokenAsync(string accessToken)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("FacebookGraph");
                var url = $"https://graph.facebook.com/{GRAPH_API_VERSION}/me?access_token={Uri.EscapeDataString(accessToken)}";

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Access token validation successful");
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Access token validation failed: {StatusCode} - {Response}", response.StatusCode, errorContent);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating access token: {Message}", ex.Message);
                return false;
            }
        }
        public async Task<bool> PostExistsAsync(string postId, string pageAccessToken)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("FacebookGraph");
                var url = $"https://graph.facebook.com/{GRAPH_API_VERSION}/{postId}?fields=id&access_token={Uri.EscapeDataString(pageAccessToken)}";

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Post {PostId} exists on Facebook", postId);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Post {PostId} does not exist on Facebook or access denied", postId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if post exists: {Message}", ex.Message);
                return false;
            }
        }
        public async Task<bool> DeletePostAsync(string postId, string pageAccessToken)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(postId))
                {
                    _logger.LogError("Post ID is null or empty");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(pageAccessToken))
                {
                    _logger.LogError("Page access token is null or empty");
                    return false;
                }

                _logger.LogInformation("Attempting to delete Facebook post {PostId}", postId);

                var client = _httpClientFactory.CreateClient("FacebookGraph");

                // Build URL with access token as query parameter
                var url = $"https://graph.facebook.com/v18.0/{postId}?access_token={Uri.EscapeDataString(pageAccessToken)}";

                _logger.LogInformation("DELETE request URL: {Url}", url.Replace(pageAccessToken, "[REDACTED]"));

                var response = await client.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Successfully deleted Facebook post {PostId}. Response: {Response}", postId, successContent);
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to delete Facebook post {PostId}. Status: {StatusCode}, Response: {Response}",
                        postId, response.StatusCode, errorContent);

                    // Try to parse Facebook error for more specific information
                    try
                    {
                        var errorObj = System.Text.Json.JsonSerializer.Deserialize<FacebookApiError>(errorContent);
                        _logger.LogError("Facebook API Error - Code: {Code}, Message: {Message}, Type: {Type}",
                            errorObj?.Error?.Code, errorObj?.Error?.Message, errorObj?.Error?.Type);
                    }
                    catch
                    {
                        // If we can't parse the error, just log the raw response
                        _logger.LogError("Could not parse Facebook error response: {ErrorContent}", errorContent);
                    }

                    return false;
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP error while deleting Facebook post {PostId}: {Message}", postId, httpEx.Message);
                return false;
            }
            catch (TaskCanceledException tcEx)
            {
                _logger.LogError(tcEx, "Request timeout while deleting Facebook post {PostId}: {Message}", postId, tcEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting Facebook post {PostId}: {Message}", postId, ex.Message);
                return false;
            }
        }
    }
}