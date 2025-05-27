using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Text;

namespace FacebookTimerPosts.Services.Repository
{
    public class ImageGenerationService : IImageGenerationService
    {
        private readonly ILogger<ImageGenerationService> _logger;
        private readonly IWebHostEnvironment _environment;

        public ImageGenerationService(
            ILogger<ImageGenerationService> logger,
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public async Task<string> GenerateCountdownImageAsync(Post post, Template template)
        {
            try
            {
                var imageBytes = await GenerateCountdownImageBytesAsync(post, template);

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "countdown");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"countdown_{post.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}.png";
                var filePath = Path.Combine(uploadsFolder, fileName);

                await File.WriteAllBytesAsync(filePath, imageBytes);

                return $"/images/countdown/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating countdown image for post {PostId}", post.Id);
                throw;
            }
        }

        public async Task<byte[]> GenerateCountdownImageBytesAsync(Post post, Template template)
        {
            try
            {
                // Download browser if not exists
                await new BrowserFetcher().DownloadAsync();

                var launchOptions = new LaunchOptions
                {
                    Headless = true,
                    Args = new[] {
                        "--no-sandbox",
                        "--disable-setuid-sandbox",
                        "--disable-dev-shm-usage",
                        "--disable-accelerated-2d-canvas",
                        "--no-first-run",
                        "--no-zygote",
                        "--disable-gpu",
                        "--force-device-scale-factor=2" // High DPI for crisp images
                    }
                };

                using var browser = await Puppeteer.LaunchAsync(launchOptions);
                using var page = await browser.NewPageAsync();

                // Set high resolution viewport for crisp images
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 1920, // Higher resolution for better quality
                    Height = 1080,
                    DeviceScaleFactor = 2 // Retina quality
                });

                // Generate professional HTML content
                var htmlContent = GenerateCountdownHtml(post, template);

                _logger.LogDebug("Generated HTML for post {PostId}", post.Id);

                // Set content and wait for everything to load
                await page.SetContentAsync(htmlContent);

                // Wait for fonts and rendering to complete
                // await page.WaitForTimeoutAsync(5000); // Increased wait time for better rendering

                // Take high-quality screenshot
                var screenshotOptions = new ScreenshotOptions
                {
                    Type = ScreenshotType.Png,
                    FullPage = false,
                    OmitBackground = false,
                    Clip = new PuppeteerSharp.Media.Clip
                    {
                        X = 0,
                        Y = 0,
                        Width = 1200, // Facebook optimal size
                        Height = 630
                    }
                };

                var imageBytes = await page.ScreenshotDataAsync(screenshotOptions);

                _logger.LogInformation("Successfully generated high-quality image for post {PostId}, size: {Size} bytes",
                    post.Id, imageBytes.Length);

                return imageBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating countdown image bytes for post {PostId}", post.Id);
                throw;
            }
        }

        private string GenerateCountdownHtml(Post post, Template template)
        {
            // Calculate countdown values
            var now = DateTime.UtcNow;
            var eventDate = post.EventDateTime;
            var timeDiff = eventDate - now;

            var days = Math.Max(0, (int)timeDiff.TotalDays);
            var hours = Math.Max(0, timeDiff.Hours);
            var minutes = Math.Max(0, timeDiff.Minutes);
            var seconds = Math.Max(0, timeDiff.Seconds);

            var isEventPassed = timeDiff.TotalMilliseconds <= 0;

            // Use template HTML if available, otherwise use enhanced default
            var htmlTemplate = !string.IsNullOrEmpty(template?.HtmlTemplate)
                ? template.HtmlTemplate
                : GetProfessionalCountdownTemplate();

            // Replace template variables
            var html = htmlTemplate
                .Replace("{{eventName}}", post.Title ?? "Special Event")
                .Replace("{{eventDescription}}", post.Description ?? "Join us for this amazing event!")
                .Replace("{{eventLink}}", "#")
                .Replace("{{buttonText}}", "REGISTER NOW")
                .Replace("{{days}}", days.ToString("00"))
                .Replace("{{hours}}", hours.ToString("00"))
                .Replace("{{minutes}}", minutes.ToString("00"))
                .Replace("{{seconds}}", seconds.ToString("00"));

            // Apply custom styling
            var customStyles = GenerateCustomStyles(post, template);

            if (html.Contains("</head>"))
            {
                html = html.Replace("</head>", $"{customStyles}</head>");
            }
            else
            {
                html = $"<html><head>{customStyles}</head><body>{html}</body></html>";
            }

            return html;
        }

        private string GenerateCustomStyles(Post post, Template template)
        {
            var fontFamily = !string.IsNullOrEmpty(post.CustomFontFamily)
                ? post.CustomFontFamily
                : template?.DefaultFontFamily ?? "'Roboto', 'Helvetica Neue', Arial, sans-serif";

            var primaryColor = !string.IsNullOrEmpty(post.CustomPrimaryColor)
                ? post.CustomPrimaryColor
                : template?.PrimaryColor ?? "#4F46E5";

            var backgroundImage = !string.IsNullOrEmpty(post.BackgroundImageUrl)
                ? post.BackgroundImageUrl
                : template?.BackgroundImageUrl;

            var styles = new StringBuilder();
            styles.AppendLine("<style>");

            // Import Google Fonts for better typography
            styles.AppendLine("@import url('https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700;900&family=Poppins:wght@300;400;500;600;700;800;900&display=swap');");

            // Base reset and body styles
            styles.AppendLine("* { margin: 0; padding: 0; box-sizing: border-box; }");
            styles.AppendLine("body {");
            styles.AppendLine($"  font-family: {fontFamily};");
            styles.AppendLine("  margin: 0;");
            styles.AppendLine("  padding: 0;");
            styles.AppendLine("  width: 1200px;");
            styles.AppendLine("  height: 630px;");
            styles.AppendLine("  display: flex;");
            styles.AppendLine("  align-items: center;");
            styles.AppendLine("  justify-content: center;");
            styles.AppendLine("  overflow: hidden;");
            styles.AppendLine("  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);");
            styles.AppendLine("  position: relative;");

            // Custom background if provided
            if (!string.IsNullOrEmpty(backgroundImage))
            {
                styles.AppendLine($"  background-image: url('{backgroundImage}');");
                styles.AppendLine("  background-size: cover;");
                styles.AppendLine("  background-position: center;");
                styles.AppendLine("  background-repeat: no-repeat;");
            }

            styles.AppendLine("}");

            // Overlay styles
            if (post.HasOverlay)
            {
                styles.AppendLine("body::before {");
                styles.AppendLine("  content: '';");
                styles.AppendLine("  position: absolute;");
                styles.AppendLine("  top: 0;");
                styles.AppendLine("  left: 0;");
                styles.AppendLine("  right: 0;");
                styles.AppendLine("  bottom: 0;");
                styles.AppendLine("  background: rgba(0,0,0,0.6);");
                styles.AppendLine("  z-index: 1;");
                styles.AppendLine("}");
            }

            // Hide countdown units based on user selection
            if (!post.ShowDays) styles.AppendLine(".countdown-days { display: none !important; }");
            if (!post.ShowHours) styles.AppendLine(".countdown-hours { display: none !important; }");
            if (!post.ShowMinutes) styles.AppendLine(".countdown-minutes { display: none !important; }");
            if (!post.ShowSeconds) styles.AppendLine(".countdown-seconds { display: none !important; }");

            styles.AppendLine("</style>");
            return styles.ToString();
        }

        private string GetProfessionalCountdownTemplate()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <style>
        .countdown-container {
            position: relative;
            text-align: center;
            color: white;
            z-index: 2;
            width: 100%;
            height: 100%;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            padding: 60px;
        }
        
        .event-badge {
            background: rgba(255,255,255,0.15);
            padding: 8px 24px;
            border-radius: 50px;
            font-size: 14px;
            font-weight: 500;
            letter-spacing: 2px;
            text-transform: uppercase;
            margin-bottom: 20px;
            backdrop-filter: blur(10px);
            border: 1px solid rgba(255,255,255,0.2);
        }
        
        .event-title {
            font-size: 56px;
            font-weight: 900;
            margin-bottom: 16px;
            text-shadow: 0 4px 20px rgba(0,0,0,0.3);
            line-height: 1.1;
            background: linear-gradient(135deg, #ffffff 0%, #f0f0f0 100%);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            background-clip: text;
        }
        
        .event-description {
            font-size: 22px;
            margin-bottom: 50px;
            text-shadow: 0 2px 10px rgba(0,0,0,0.3);
            line-height: 1.4;
            max-width: 800px;
            opacity: 0.95;
            font-weight: 400;
        }
        
        .countdown-timer {
            display: flex;
            justify-content: center;
            align-items: center;
            gap: 20px;
            flex-wrap: wrap;
            margin-bottom: 40px;
        }
        
        .countdown-unit {
            text-align: center;
            background: linear-gradient(145deg, rgba(255,255,255,0.25), rgba(255,255,255,0.1));
            padding: 30px 25px;
            border-radius: 20px;
            backdrop-filter: blur(20px);
            border: 2px solid rgba(255,255,255,0.3);
            min-width: 140px;
            box-shadow: 0 8px 32px rgba(0,0,0,0.2);
            transition: all 0.3s ease;
        }
        
        .countdown-unit:hover {
            transform: translateY(-5px);
            box-shadow: 0 12px 40px rgba(0,0,0,0.3);
        }
        
        .countdown-value {
            font-size: 72px;
            font-weight: 900;
            line-height: 1;
            margin-bottom: 8px;
            color: white;
            text-shadow: 0 4px 15px rgba(0,0,0,0.3);
        }
        
        .countdown-label {
            font-size: 16px;
            opacity: 0.9;
            text-transform: uppercase;
            letter-spacing: 3px;
            font-weight: 600;
            color: rgba(255,255,255,0.9);
        }
        
        .countdown-separator {
            font-size: 48px;
            font-weight: 300;
            color: rgba(255,255,255,0.6);
            opacity: 0.8;
        }
        
        .register-button {
            background: linear-gradient(135deg, #FF6B6B 0%, #FF8E53 100%);
            color: white;
            padding: 18px 50px;
            border: none;
            border-radius: 50px;
            font-size: 18px;
            font-weight: 700;
            letter-spacing: 1px;
            text-transform: uppercase;
            cursor: pointer;
            box-shadow: 0 10px 30px rgba(255,107,107,0.4);
            transition: all 0.3s ease;
            text-decoration: none;
            display: inline-block;
        }
        
        .register-button:hover {
            transform: translateY(-3px);
            box-shadow: 0 15px 40px rgba(255,107,107,0.6);
        }
        
        .decorative-elements {
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            pointer-events: none;
            z-index: 1;
        }
        
        .decorative-circle {
            position: absolute;
            border-radius: 50%;
            background: radial-gradient(circle, rgba(255,255,255,0.1) 0%, transparent 70%);
        }
        
        .circle-1 {
            width: 300px;
            height: 300px;
            top: -150px;
            right: -150px;
        }
        
        .circle-2 {
            width: 200px;
            height: 200px;
            bottom: -100px;
            left: -100px;
        }
        
        .circle-3 {
            width: 150px;
            height: 150px;
            top: 20%;
            left: 10%;
            opacity: 0.5;
        }
    </style>
</head>
<body>
    <div class='decorative-elements'>
        <div class='decorative-circle circle-1'></div>
        <div class='decorative-circle circle-2'></div>
        <div class='decorative-circle circle-3'></div>
    </div>
    
    <div class='countdown-container'>
        <div class='event-badge'>Special Event</div>
        <div class='event-title'>{{eventName}}</div>
        <div class='event-description'>{{eventDescription}}</div>
        
        <div class='countdown-timer'>
            <div class='countdown-unit countdown-days'>
                <div class='countdown-value'>{{days}}</div>
                <div class='countdown-label'>Days</div>
            </div>
            <div class='countdown-separator'>:</div>
            <div class='countdown-unit countdown-hours'>
                <div class='countdown-value'>{{hours}}</div>
                <div class='countdown-label'>Hours</div>
            </div>
            <div class='countdown-separator'>:</div>
            <div class='countdown-unit countdown-minutes'>
                <div class='countdown-value'>{{minutes}}</div>
                <div class='countdown-label'>Minutes</div>
            </div>
            <div class='countdown-separator'>:</div>
            <div class='countdown-unit countdown-seconds'>
                <div class='countdown-value'>{{seconds}}</div>
                <div class='countdown-label'>Seconds</div>
            </div>
        </div>
        
        <a href='{{eventLink}}' class='register-button'>{{buttonText}}</a>
    </div>
</body>
</html>";
        }
    }
}