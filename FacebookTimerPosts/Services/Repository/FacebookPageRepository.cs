using FacebookTimerPosts.AppDbContext;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using FacebookTimerPosts.Services.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace FacebookTimerPosts.Services.Repository
{
    public class FacebookPageRepository : Repository<FacebookPage>, IFacebookPageRepository
    {
        private readonly HttpClient _httpClient;

        public FacebookPageRepository(ApplicationDbContext context, HttpClient httpClient) : base(context)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://graph.facebook.com/v17.0/");
        }

        public async Task<IEnumerable<FacebookPage>> GetUserPagesAsync(int userId)
        {
            return await _context.FacebookPages
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<FacebookPage> GetPageWithPostsAsync(int pageId)
        {
            return await _context.FacebookPages
                .Include(p => p.Posts)
                .FirstOrDefaultAsync(p => p.Id == pageId);
        }

        public async Task<bool> LinkPageToUserAsync(FacebookPage page, int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return false;

            page.UserId = userId;
            await _context.FacebookPages.AddAsync(page);

            return await SaveAllAsync();
        }

        public async Task<bool> UnlinkPageFromUserAsync(int pageId, int userId)
        {
            var page = await _context.FacebookPages
                .FirstOrDefaultAsync(p => p.Id == pageId && p.UserId == userId);

            if (page == null) return false;

            _context.FacebookPages.Remove(page);

            return await SaveAllAsync();
        }

        public async Task<FacebookPage> GetPageByFacebookIdAsync(string facebookPageId)
        {
            return await _context.FacebookPages
                .FirstOrDefaultAsync(p => p.FacebookPageId == facebookPageId);
        }

        public async Task<PageDetails> GetPageDetailsAsync(string pageId, string accessToken)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<PageDetailsResponse>(
                    $"{pageId}?fields=id,name,picture&access_token={accessToken}");

                if (response == null) return null;

                return new PageDetails
                {
                    Id = response.Id,
                    Name = response.Name,
                    PictureUrl = response.Picture?.Data?.Url
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> CreatePostAsync(string pageId, string accessToken, string message)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("message", message),
                new KeyValuePair<string, string>("access_token", accessToken)
            });

                var response = await _httpClient.PostAsync($"{pageId}/feed", content);

                if (!response.IsSuccessStatusCode) return null;

                var result = await response.Content.ReadFromJsonAsync<FacebookPostResponse>();

                return result?.Id;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }

    public class PageDetailsResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public PictureResponse Picture { get; set; }
    }

    public class PictureResponse
    {
        public PictureDataResponse Data { get; set; }
    }

    public class PictureDataResponse
    {
        public string Url { get; set; }
    }

    public class FacebookPostResponse
    {
        public string Id { get; set; }
    }

    public class PageDetails
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
    }
}
