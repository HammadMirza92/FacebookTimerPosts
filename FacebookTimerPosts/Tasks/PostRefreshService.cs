using FacebookTimerPosts.Enums;
using FacebookTimerPosts.Services.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FacebookTimerPosts.Services
{
    public class PostRefreshService : BackgroundService
    {
        private readonly ILogger<PostRefreshService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Timer? _timer;

        public PostRefreshService(ILogger<PostRefreshService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Post Refresh Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(5)); // Check every 5 minutes

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            _logger.LogInformation("Post Refresh Service is working.");

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var postRepository = scope.ServiceProvider.GetRequiredService<IPostRepository>();
                    var facebookService = scope.ServiceProvider.GetRequiredService<IFacebookService>();
                    var countdownTimerRepository = scope.ServiceProvider.GetRequiredService<ICountdownTimerRepository>();

                    var now = DateTime.UtcNow;
                    var postsToRefresh = await postRepository.GetPostsDueForRefresh(now);

                    _logger.LogInformation("Found {Count} posts due for refresh", postsToRefresh.Count);

                    foreach (var post in postsToRefresh)
                    {
                        try
                        {
                            // Check if post is still valid for refresh (event hasn't passed yet)
                            if (post.EventDateTime <= now)
                            {
                                _logger.LogInformation("Post {PostId} event has passed, skipping refresh", post.Id);
                                continue;
                            }

                            // Get countdown image URL
                            var countdownTimer = await countdownTimerRepository.GetByPostIdAsync(post.Id);
                            if (countdownTimer == null)
                            {
                                _logger.LogWarning("No countdown timer found for post {PostId}", post.Id);
                                continue;
                            }

                            var imageUrl = $"{GetBaseUrl()}/api/countdown/{countdownTimer.PublicId}/image";

                            // Call Facebook service to publish the post (it will replace the existing one)
                            var result = await facebookService.PublishPostAsync(post, imageUrl);

                            if (result.Success)
                            {
                                // Calculate next refresh time
                                var nextRefreshTime = now.AddMinutes(post.RefreshIntervalInMinutes.Value);

                                // Update post in database with new Facebook post ID and next refresh time
                                await postRepository.UpdatePostRefreshAsync(post.Id, result.PostId, nextRefreshTime);

                                _logger.LogInformation("Successfully refreshed post {PostId}, next refresh at {NextRefresh}",
                                    post.Id, nextRefreshTime);
                            }
                            else
                            {
                                _logger.LogError("Failed to refresh post {PostId}: {ErrorMessage}", post.Id, result.ErrorMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error refreshing post {PostId}", post.Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in post refresh service");
            }
        }

        private string GetBaseUrl()
        {
            // This would come from configuration in a real application
            return "https://yourdomain.com";
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Post Refresh Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return base.StopAsync(stoppingToken);
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}