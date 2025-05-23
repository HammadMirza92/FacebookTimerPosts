﻿using FacebookTimerPosts.Enums;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FacebookTimerPosts.Services
{
    public class ScheduledTaskService : BackgroundService
    {
        private readonly ILogger<ScheduledTaskService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Timer? _timer;

        public ScheduledTaskService(ILogger<ScheduledTaskService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scheduled Task Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            _logger.LogInformation("Scheduled Task Service is working.");

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var postRepository = scope.ServiceProvider.GetRequiredService<IPostRepository>();
                    var facebookService = scope.ServiceProvider.GetRequiredService<IFacebookService>();
                    var countdownTimerRepository = scope.ServiceProvider.GetRequiredService<ICountdownTimerRepository>();

                    var now = DateTime.UtcNow;
                    var scheduledPosts = await postRepository.GetDueScheduledPosts(now);

                    _logger.LogInformation("Found {Count} posts due for publishing", scheduledPosts.Count);

                    foreach (var post in scheduledPosts)
                    {
                        try
                        {
                            // Get countdown image URL
                            var countdownTimer = await countdownTimerRepository.GetByPostIdAsync(post.Id);
                            if (countdownTimer == null)
                            {
                                _logger.LogWarning("No countdown timer found for post {PostId}", post.Id);
                                continue;
                            }

                            var imageUrl = $"{GetBaseUrl()}/api/countdown/{countdownTimer.PublicId}/image";

                            // Call Facebook service to publish the post
                            var result = await facebookService.PublishPostAsync(post, imageUrl);

                            if (result.Success)
                            {
                                // Update post in database
                                await postRepository.UpdatePostStatusAsync(post.Id, PostStatus.Published, result.PostId);
                                _logger.LogInformation("Successfully published post {PostId} to Facebook", post.Id);
                            }
                            else
                            {
                                _logger.LogError("Failed to publish post {PostId} to Facebook: {ErrorMessage}", post.Id, result.ErrorMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error publishing post {PostId}", post.Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in scheduled task service");
            }
        }

        private string GetBaseUrl()
        {
            // This would come from configuration in a real application
            return "https://yourdomain.com";
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scheduled Task Service is stopping.");

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