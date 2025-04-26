namespace FacebookTimerPosts.Tasks
{
    public class ScheduledTaskService : BackgroundService
    {
        private readonly ILogger<ScheduledTaskService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _period = TimeSpan.FromMinutes(1);

        public ScheduledTaskService(
            ILogger<ScheduledTaskService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(_period);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await ProcessScheduledPosts();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing scheduled posts");
                }
            }
        }

        private async Task ProcessScheduledPosts()
        {
            _logger.LogInformation("Processing scheduled posts at: {time}", DateTimeOffset.Now);

            // Create a scope for the scoped services
            using var scope = _serviceProvider.CreateScope();
            var httpClient = new HttpClient();
            var baseUrl = _configuration["ApplicationUrl"] ?? "https://localhost:7001";
            var apiKey = _configuration["BackgroundTaskApiKey"] ?? "your-secure-api-key-for-background-tasks";

            try
            {
                httpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
                var response = await httpClient.PostAsync($"{baseUrl}/api/BackgroundTask/process-scheduled-posts", null);

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Scheduled post processing result: {result}", content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing scheduled posts");
            }
        }
    }
}