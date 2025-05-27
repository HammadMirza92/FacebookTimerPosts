using FacebookTimerPosts.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FacebookTimerPosts.AppDbContext
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<FacebookPage> FacebookPages { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<CountdownTimer> CountdownTimers { get; set; }
        public DbSet<PaymentResult> PaymentResults { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<FacebookPostResult> FacebookPostResults { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure User
            builder.Entity<User>()
                .HasMany(u => u.UserSubscriptions)
                .WithOne(us => us.User)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<User>()
                .HasMany(u => u.FacebookPages)
                .WithOne(fp => fp.User)
                .HasForeignKey(fp => fp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //builder.Entity<User>()
            //    .HasMany(u => u.Posts)
            //    .WithOne(p => p.User)
            //    .HasForeignKey(p => p.UserId)
            //    .OnDelete(DeleteBehavior.NoAction);

            // Configure SubscriptionPlan
            builder.Entity<SubscriptionPlan>()
                .HasMany(sp => sp.UserSubscriptions)
                .WithOne(us => us.SubscriptionPlan)
                .HasForeignKey(us => us.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Template
            builder.Entity<Template>()
                .HasOne(t => t.MinimumSubscriptionPlan)
                .WithMany(sp => sp.AvailableTemplates)
                .HasForeignKey(t => t.MinimumSubscriptionPlanId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Template>()
                .HasMany(t => t.Posts)
                .WithOne(p => p.Template)
                .HasForeignKey(p => p.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure FacebookPage
            builder.Entity<FacebookPage>()
                .HasMany(fp => fp.Posts)
                .WithOne(p => p.FacebookPage)
                .HasForeignKey(p => p.FacebookPageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Post
            builder.Entity<Post>()
                .Property(p => p.Status)
                .HasConversion<string>();

            // Add configuration for RefreshIntervalInMinutes and NextRefreshTime
            builder.Entity<Post>()
                .Property(p => p.RefreshIntervalInMinutes)
                .IsRequired(false);

            builder.Entity<Post>()
                .Property(p => p.NextRefreshTime)
                .IsRequired(false);

            // Configure CountdownTimer
            builder.Entity<CountdownTimer>()
                .HasOne(ct => ct.Post)
                .WithMany()
                .HasForeignKey(ct => ct.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure PaymentResult
            builder.Entity<PaymentResult>()
                .Property(pr => pr.Status)
                .HasConversion<string>();

            builder.Entity<PaymentResult>()
                .HasOne(pr => pr.UserSubscription)
                .WithMany()
                .HasForeignKey(pr => pr.UserSubscriptionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed initial data
            SeedData(builder);
        }

        private void SeedData(ModelBuilder builder)
        {
            // Seed subscription plans
            builder.Entity<SubscriptionPlan>().HasData(
                new SubscriptionPlan
                {
                    Id = 1,
                    Name = "Free",
                    Description = "Free plan with basic features",
                    Price = 0,
                    DurationInDays = 0,
                    MaxPosts = 1,
                    MaxTemplates = 7,
                    IsActive = true,
                    CreatedAt = System.DateTime.UtcNow
                },
                new SubscriptionPlan
                {
                    Id = 2,
                    Name = "Pro",
                    Description = "Professional plan with advanced features",
                    Price = 20,
                    DurationInDays = 30,
                    MaxPosts = 10,
                    MaxTemplates = 20,
                    IsActive = true,
                    CreatedAt = System.DateTime.UtcNow
                },
                new SubscriptionPlan
                {
                    Id = 3,
                    Name = "Premium",
                    Description = "Premium plan with all features",
                    Price = 50,
                    DurationInDays = 30,
                    MaxPosts = 20,
                    MaxTemplates = 0, // Unlimited
                    IsActive = true,
                    CreatedAt = System.DateTime.UtcNow
                }
            );

            // Seed templates
            builder.Entity<Template>().HasData(
              new Template
              {
                  Id= 1,
                  Name = "Classic Dark",
                  Description = "A simple dark-themed countdown timer",
                  DefaultFontFamily = "Arial, sans-serif",
                  PrimaryColor = "#1a1a1a",
                  RequiresSubscription = false,
                  IsActive = true,
                  CreatedAt = DateTime.UtcNow,
                  HtmlTemplate = GetClassicDarkTemplate()
              },
                    new Template
                    {
                        Id = 2,
                        Name = "Simple Light",
                        Description = "A clean light-themed countdown timer",
                        DefaultFontFamily = "Roboto, sans-serif",
                        PrimaryColor = "#f5f5f5",
                        RequiresSubscription = false,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        HtmlTemplate = GetSimpleLightTemplate()
                    },
                    new Template
                    {
                        Id = 3,
                        Name = "Get Ready",
                        Description = "A vibrant countdown with eye-catching colors",
                        DefaultFontFamily = "Montserrat, sans-serif",
                        PrimaryColor = "#e91e63",
                        RequiresSubscription = false,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        HtmlTemplate = GetReadyTemplate()
                    },
                    new Template
                    {
                        Id = 4,
                        Name = "Minimal",
                        Description = "A minimalist countdown timer design",
                        DefaultFontFamily = "Helvetica, sans-serif",
                        PrimaryColor = "#333333",
                        RequiresSubscription = false,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        HtmlTemplate = GetMinimalTemplate()
                    },

                    // Pro Subscription Templates
                    new Template
                    {
                        Id = 5,
                        Name = "Special Promo",
                        Description = "A professional design for promotional events",
                        DefaultFontFamily = "Poppins, sans-serif",
                        PrimaryColor = "#8e44ad",
                        RequiresSubscription = true,
                        MinimumSubscriptionPlanId = 2, // Pro plan
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        HtmlTemplate = GetSpecialPromoTemplate()
                    },
                    new Template
                    {
                        Id = 6,
                        Name = "Teal Business",
                        Description = "A professional teal-themed countdown for business events",
                        DefaultFontFamily = "Lato, sans-serif",
                        PrimaryColor = "#009688",
                        RequiresSubscription = true,
                        MinimumSubscriptionPlanId = 2, // Pro plan
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        HtmlTemplate = GetTealBusinessTemplate()
                    },

                    // Premium Subscription Templates
                    new Template
                    {
                        Id = 7,
                        Name = "Black Friday",
                        Description = "A high-impact sales countdown for Black Friday events",
                        DefaultFontFamily = "Oswald, sans-serif",
                        PrimaryColor = "#000000",
                        RequiresSubscription = true,
                        MinimumSubscriptionPlanId = 3, // Premium plan
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        HtmlTemplate = GetBlackFridayTemplate()
                    },
                    new Template
                    {
                        Id = 8,
                        Name = "Launching Soon",
                        Description = "A sleek launch countdown for product releases",
                        DefaultFontFamily = "Raleway, sans-serif",
                        PrimaryColor = "#2c3e50",
                        RequiresSubscription = true,
                        MinimumSubscriptionPlanId = 3, // Premium plan
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        HtmlTemplate = GetLaunchingSoonTemplate()
                    }
            );
        }

             private static string GetClassicDarkTemplate()
        {
            return @"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <title>Countdown Timer Post</title>
  <style>
    .countdown-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      height: 100%;
      width: 100%;
      padding: 20px;
      box-sizing: border-box;
      font-family: Arial, sans-serif;
      background-color: #1a1a1a;
      color: white;
      text-align: center;
    }
    
    .event-title {
      font-size: 32px;
      font-weight: bold;
      margin-bottom: 30px;
      text-transform: uppercase;
    }
    
    .countdown-timer {
      display: flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 30px;
    }
    
    .time-block {
      display: flex;
      flex-direction: column;
      align-items: center;
      margin: 0 5px;
    }
    
    .time-value {
      font-size: 48px;
      font-weight: bold;
      background-color: #333;
      border-radius: 8px;
      padding: 10px;
      min-width: 80px;
    }
    
    .time-label {
      margin-top: 10px;
      font-size: 14px;
      text-transform: uppercase;
      letter-spacing: 1px;
    }
    
    .time-separator {
      font-size: 48px;
      margin: 0 5px;
      padding-bottom: 25px;
    }
    
    .event-details {
      max-width: 500px;
    }
    
    .event-description {
      margin-bottom: 20px;
      font-size: 16px;
      line-height: 1.5;
    }
    
    .event-button {
      display: inline-block;
      padding: 12px 30px;
      background-color: #ff4500;
      color: white;
      text-decoration: none;
      border-radius: 30px;
      font-weight: bold;
      text-transform: uppercase;
      letter-spacing: 1px;
      transition: all 0.3s ease;
    }
    
    .event-button:hover {
      background-color: #ff6a33;
      transform: translateY(-2px);
      box-shadow: 0 5px 15px rgba(255, 69, 0, 0.4);
    }
  </style>
</head>
<body>
<div class=""countdown-container"">
  <h1 class=""event-title"">{{eventName}}</h1>
  
  <div class=""countdown-timer"">
    <div class=""time-block"">
      <div class=""time-value"">{{days}}</div>
      <div class=""time-label"">Days</div>
    </div>
    <div class=""time-separator"">:</div>
    <div class=""time-block"">
      <div class=""time-value"">{{hours}}</div>
      <div class=""time-label"">Hours</div>
    </div>
    <div class=""time-separator"">:</div>
    <div class=""time-block"">
      <div class=""time-value"">{{minutes}}</div>
      <div class=""time-label"">Minutes</div>
    </div>
    <div class=""time-separator"">:</div>
    <div class=""time-block"">
      <div class=""time-value"">{{seconds}}</div>
      <div class=""time-label"">Seconds</div>
    </div>
  </div>
  
  <div class=""event-details"">
    <p class=""event-description"">{{eventDescription}}</p>
    <a href=""{{eventLink}}"" class=""event-button"">{{buttonText}}</a>
  </div>
</div>
</body>
</html>";
        }

        private static string GetSimpleLightTemplate()
        {
            return @"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <title>Simple Light Countdown</title>
  <style>
    .countdown-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      height: 100%;
      width: 100%;
      padding: 20px;
      box-sizing: border-box;
      font-family: Roboto, sans-serif;
      background-color: #f5f5f5;
      color: #333;
      text-align: center;
    }
    
    .event-title {
      font-size: 28px;
      font-weight: 500;
      margin-bottom: 25px;
    }
    
    .countdown-timer {
      display: flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 25px;
    }
    
    .time-block {
      display: flex;
      flex-direction: column;
      align-items: center;
      margin: 0 10px;
    }
    
    .time-value {
      font-size: 40px;
      font-weight: bold;
      color: #333;
      background-color: #fff;
      border: 1px solid #ddd;
      border-radius: 4px;
      padding: 10px;
      min-width: 60px;
      box-shadow: 0 2px 5px rgba(0,0,0,0.1);
    }
    
    .time-label {
      margin-top: 8px;
      font-size: 12px;
      color: #666;
      text-transform: uppercase;
    }
    
    .event-details {
      max-width: 500px;
    }
    
    .event-description {
      margin-bottom: 20px;
      font-size: 16px;
      line-height: 1.5;
      color: #555;
    }
    
    .event-button {
      display: inline-block;
      padding: 10px 25px;
      background-color: #2196F3;
      color: white;
      text-decoration: none;
      border-radius: 4px;
      font-weight: 500;
      transition: all 0.2s ease;
    }
    
    .event-button:hover {
      background-color: #1976D2;
      box-shadow: 0 2px 8px rgba(33, 150, 243, 0.4);
    }
  </style>
</head>
<body>
<div class=""countdown-container"">
  <h1 class=""event-title"">{{eventName}}</h1>
  
  <div class=""countdown-timer"">
    <div class=""time-block"">
      <div class=""time-value"">{{days}}</div>
      <div class=""time-label"">Days</div>
    </div>
    <div class=""time-block"">
      <div class=""time-value"">{{hours}}</div>
      <div class=""time-label"">Hours</div>
    </div>
    <div class=""time-block"">
      <div class=""time-value"">{{minutes}}</div>
      <div class=""time-label"">Minutes</div>
    </div>
    <div class=""time-block"">
      <div class=""time-value"">{{seconds}}</div>
      <div class=""time-label"">Seconds</div>
    </div>
  </div>
  
  <div class=""event-details"">
    <p class=""event-description"">{{eventDescription}}</p>
    <a href=""{{eventLink}}"" class=""event-button"">{{buttonText}}</a>
  </div>
</div>
</body>
</html>";
        }

        private static string GetReadyTemplate()
        {
            return @"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <title>Get Ready Countdown</title>
  <style>
    .countdown-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      height: 100%;
      width: 100%;
      padding: 20px;
      box-sizing: border-box;
      font-family: Montserrat, sans-serif;
      background: linear-gradient(135deg, #e91e63 0%, #5d02ff 100%);
      color: white;
      text-align: center;
    }
    
    .event-header {
      margin-bottom: 20px;
      background-color: rgba(255, 255, 255, 0.15);
      padding: 10px 20px;
      border-radius: 30px;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 2px;
      font-size: 16px;
    }
    
    .hours-block {
      display: flex;
      flex-direction: column;
      align-items: center;
      margin: 0 auto 20px;
      background-color: white;
      color: #333;
      height: 150px;
      width: 150px;
      border-radius: 50%;
      display: flex;
      justify-content: center;
      align-items: center;
      box-shadow: 0 10px 20px rgba(0,0,0,0.2);
    }
    
    .hours-value {
      font-size: 50px;
      font-weight: 700;
      line-height: 1;
    }
    
    .hours-label {
      font-size: 16px;
      text-transform: uppercase;
      font-weight: 500;
      color: #e91e63;
    }
    
    .event-title {
      font-size: 28px;
      font-weight: 700;
      margin: 20px 0;
      text-transform: uppercase;
      letter-spacing: 1px;
    }
    
    .event-description {
      margin-bottom: 30px;
      font-size: 16px;
      line-height: 1.6;
      max-width: 600px;
    }
    
    .event-button {
      display: inline-block;
      padding: 12px 30px;
      background-color: white;
      color: #e91e63;
      text-decoration: none;
      border-radius: 30px;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 1px;
      transition: all 0.3s ease;
      box-shadow: 0 5px 15px rgba(0,0,0,0.2);
    }
    
    .event-button:hover {
      transform: translateY(-3px);
      box-shadow: 0 8px 20px rgba(0,0,0,0.3);
    }
  </style>
</head>
<body>
<div class=""countdown-container"">
  <div class=""event-header"">GET READY!</div>
  
  <div class=""hours-block"">
    <div class=""hours-value"">{{hours}}</div>
    <div class=""hours-label"">Hours</div>
  </div>
  
  <h1 class=""event-title"">{{eventName}}</h1>
  
  <p class=""event-description"">{{eventDescription}}</p>
  
  <a href=""{{eventLink}}"" class=""event-button"">{{buttonText}}</a>
</div>
</body>
</html>";
        }

        private static string GetMinimalTemplate()
        {
            return @"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <title>Minimal Countdown</title>
  <style>
    .countdown-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      height: 100%;
      width: 100%;
      padding: 20px;
      box-sizing: border-box;
      font-family: Helvetica, sans-serif;
      background-color: #ffffff;
      color: #333333;
      text-align: center;
    }
    
    .event-title {
      font-size: 24px;
      font-weight: 400;
      margin-bottom: 40px;
    }
    
    .countdown-timer {
      display: flex;
      align-items: baseline;
      justify-content: center;
      margin-bottom: 40px;
      font-size: 60px;
      font-weight: 300;
      letter-spacing: -2px;
    }
    
    .time-separator {
      margin: 0 5px;
      color: #cccccc;
      font-weight: 300;
    }
    
    .event-button {
      display: inline-block;
      padding: 12px 30px;
      background-color: #333333;
      color: white;
      text-decoration: none;
      border-radius: 2px;
      font-size: 14px;
      transition: all 0.2s ease;
    }
    
    .event-button:hover {
      background-color: #000000;
    }
  </style>
</head>
<body>
<div class=""countdown-container"">
  <h1 class=""event-title"">{{eventName}}</h1>
  
  <div class=""countdown-timer"">
    {{days}}
    <span class=""time-separator"">:</span>
    {{hours}}
    <span class=""time-separator"">:</span>
    {{minutes}}
    <span class=""time-separator"">:</span>
    {{seconds}}
  </div>
  
  <a href=""{{eventLink}}"" class=""event-button"">{{buttonText}}</a>
</div>
</body>
</html>";
        }

        private static string GetSpecialPromoTemplate()
        {
            return @"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <title>Special Promo Countdown</title>
  <style>
    .countdown-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      height: 100%;
      width: 100%;
      padding: 30px;
      box-sizing: border-box;
      font-family: Poppins, sans-serif;
      background: linear-gradient(135deg, #8e44ad 0%, #3498db 100%);
      color: white;
      text-align: center;
      border-radius: 10px;
      overflow: hidden;
      position: relative;
    }
    
    .promo-label {
      position: absolute;
      top: 20px;
      right: -35px;
      background-color: #f39c12;
      color: white;
      padding: 5px 40px;
      font-size: 12px;
      transform: rotate(45deg);
      font-weight: 600;
      box-shadow: 0 2px 5px rgba(0,0,0,0.2);
      z-index: 10;
    }
    
    .event-title {
      font-size: 26px;
      font-weight: 600;
      margin-bottom: 10px;
      text-transform: uppercase;
      letter-spacing: 1px;
    }
    
    .event-subtitle {
      font-size: 14px;
      opacity: 0.8;
      margin-bottom: 30px;
    }
    
    .countdown-timer {
      display: flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 30px;
      width: 100%;
      max-width: 500px;
    }
    
    .time-block {
      flex: 1;
      margin: 0 5px;
      background-color: rgba(255, 255, 255, 0.1);
      border-radius: 8px;
      padding: 15px 5px;
    }
    
    .time-value {
      font-size: 36px;
      font-weight: 700;
      margin-bottom: 5px;
    }
    
    .time-label {
      font-size: 11px;
      text-transform: uppercase;
      letter-spacing: 1px;
      opacity: 0.8;
    }
    
    .event-details {
      background-color: rgba(255, 255, 255, 0.1);
      padding: 20px;
      border-radius: 8px;
      max-width: 500px;
      margin-bottom: 20px;
    }
    
    .event-description {
      margin-bottom: 20px;
      font-size: 14px;
      line-height: 1.6;
    }
    
    .promo-code {
      background-color: white;
      color: #8e44ad;
      padding: 10px 20px;
      font-weight: 600;
      border-radius: 5px;
      margin-bottom: 20px;
      letter-spacing: 2px;
      display: inline-block;
    }
    
    .event-button {
      display: inline-block;
      padding: 12px 30px;
      background-color: #f39c12;
      color: white;
      text-decoration: none;
      border-radius: 30px;
      font-weight: 600;
      transition: all 0.3s ease;
      box-shadow: 0 4px 10px rgba(0,0,0,0.2);
    }
    
    .event-button:hover {
      background-color: #e67e22;
      transform: translateY(-2px);
      box-shadow: 0 6px 15px rgba(0,0,0,0.3);
    }
  </style>
</head>
<body>
<div class=""countdown-container"">
  <div class=""promo-label"">SPECIAL PROMO</div>
  
  <h1 class=""event-title"">{{eventName}}</h1>
  <div class=""event-subtitle"">Limited time offer - Don't miss out!</div>
  
  <div class=""countdown-timer"">
    <div class=""time-block"">
      <div class=""time-value"">{{days}}</div>
      <div class=""time-label"">Days</div>
    </div>
    <div class=""time-block"">
      <div class=""time-value"">{{hours}}</div>
      <div class=""time-label"">Hours</div>
    </div>
    <div class=""time-block"">
      <div class=""time-value"">{{minutes}}</div>
      <div class=""time-label"">Minutes</div>
    </div>
    <div class=""time-block"">
      <div class=""time-value"">{{seconds}}</div>
      <div class=""time-label"">Seconds</div>
    </div>
  </div>
  
  <div class=""event-details"">
    <p class=""event-description"">{{eventDescription}}</p>
    <div class=""promo-code"">PROMO25</div>
  </div>
  
  <a href=""{{eventLink}}"" class=""event-button"">{{buttonText}}</a>
</div>
</body>
</html>";
        }

        private static string GetTealBusinessTemplate()
        {
            return @"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <title>Teal Business Countdown</title>
  <style>
    .countdown-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      height: 100%;
      width: 100%;
      padding: 30px;
      box-sizing: border-box;
      font-family: Lato, sans-serif;
      background-color: #009688;
      color: white;
      text-align: center;
    }
    
    .event-title {
      font-size: 28px;
      font-weight: 300;
      margin-bottom: 5px;
      text-transform: uppercase;
      letter-spacing: 2px;
    }
    
    .event-subtitle {
      font-size: 16px;
      font-weight: 300;
      margin-bottom: 30px;
      opacity: 0.9;
    }
    
    .countdown-timer {
      display: flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 30px;
    }
    
    .time-block {
      position: relative;
      min-width: 80px;
      margin: 0 10px;
    }
    
    .time-value {
      font-size: 54px;
      font-weight: 300;
      display: block;
      line-height: 1;
    }
    
    .time-label {
      position: absolute;
      bottom: -20px;
      left: 0;
      right: 0;
      text-align: center;
      font-size: 11px;
      text-transform: uppercase;
      letter-spacing: 1px;
      opacity: 0.8;
    }
    
    .countdown-footer {
      width: 100%;
      max-width: 600px;
      margin-top: 40px;
      padding-top: 20px;
      border-top: 1px solid rgba(255, 255, 255, 0.3);
    }
    
    .event-description {
      margin-bottom: 20px;
      font-size: 16px;
      line-height: 1.6;
      font-weight: 300;
    }
    
    .event-button {
      display: inline-block;
      padding: 12px 30px;
      background-color: white;
      color: #009688;
      text-decoration: none;
      border-radius: 3px;
      font-weight: 700;
      transition: all 0.3s ease;
      text-transform: uppercase;
      letter-spacing: 1px;
      font-size: 12px;
    }
    
    .event-button:hover {
      background-color: rgba(255, 255, 255, 0.9);
      transform: translateY(-2px);
      box-shadow: 0 4px 10px rgba(0,0,0,0.1);
    }
  </style>
</head>
<body>
<div class=""countdown-container"">
  <h1 class=""event-title"">{{eventName}}</h1>
  <div class=""event-subtitle"">Join us for this exclusive event</div>
  
  <div class=""countdown-timer"">
    <div class=""time-block"">
      <span class=""time-value"">{{days}}</span>
      <span class=""time-label"">Days</span>
    </div>
    
    <div class=""time-block"">
      <span class=""time-value"">{{hours}}</span>
      <span class=""time-label"">Hours</span>
    </div>
    
    <div class=""time-block"">
      <span class=""time-value"">{{minutes}}</span>
      <span class=""time-label"">Minutes</span>
    </div>
    
    <div class=""time-block"">
      <span class=""time-value"">{{seconds}}</span>
      <span class=""time-label"">Seconds</span>
    </div>
  </div>
  
  <div class=""countdown-footer"">
    <p class=""event-description"">{{eventDescription}}</p>
    <a href=""{{eventLink}}"" class=""event-button"">{{buttonText}}</a>
  </div>
</div>
</body>
</html>";
        }

        private static string GetBlackFridayTemplate()
        {
            return @"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <title>Black Friday Countdown</title>
  <style>
    @keyframes pulse {
      0% { transform: scale(1); }
      50% { transform: scale(1.05); }
      100% { transform: scale(1); }
    }
    
    .countdown-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      height: 100%;
      width: 100%;
      padding: 20px;
      box-sizing: border-box;
      font-family: Oswald, sans-serif;
      background-color: #000000;
      color: #ffffff;
      text-align: center;
      background-image: url('data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSI1IiBoZWlnaHQ9IjUiPgo8cmVjdCB3aWR0aD0iNSIgaGVpZ2h0PSI1IiBmaWxsPSIjMDAwIj48L3JlY3Q+CjxwYXRoIGQ9Ik0wIDVMNSAwWk02IDRMNCA2Wk0tMSAxTDEgLTFaIiBzdHJva2U9IiMyMjIiIHN0cm9rZS13aWR0aD0iMSI+PC9wYXRoPgo8L3N2Zz4=');
    }
    
    .bf-header {
      margin-bottom: 20px;
    }
    
    .bf-title {
      font-size: 40px;
      font-weight: 700;
      text-transform: uppercase;
      line-height: 1;
      margin: 0;
      text-shadow: 0 0 10px rgba(255, 255, 255, 0.3);
    }
    
    .bf-title span {
      color: #aaff00;
      display: block;
      font-size: 60px;
      animation: pulse 2s infinite;
    }
    
    .countdown-timer {
      display: flex;
      align-items: center;
      justify-content: center;
      margin: 30px 0;
    }
    
    .time-block {
      background-color: #1a1a1a;
      border: 2px solid #aaff00;
      padding: 15px 5px;
      margin: 0 5px;
      min-width: 80px;
      box-shadow: 0 0 15px rgba(170, 255, 0, 0.3);
    }
    
    .time-value {
      font-size: 50px;
      font-weight: 700;
      color: #aaff00;
      line-height: 1;
    }
    
    .time-label {
      margin-top: 10px;
      font-size: 14px;
      text-transform: uppercase;
      color: #ffffff;
    }
    
    .days-remaining {
      font-size: 24px;
      text-transform: uppercase;
      margin-bottom: 30px;
    }
    
    .days-remaining strong {
      color: #aaff00;
    }
    
    .discount-badge {
      background-color: #aaff00;
      color: #000000;
      font-size: 28px;
      font-weight: 700;
      padding: 10px 20px;
      border-radius: 5px;
      margin-bottom: 20px;
      display: inline-block;
      text-transform: uppercase;
    }
    
    .event-description {
      font-size: 16px;
      line-height: 1.5;
      margin-bottom: 30px;
      max-width: 600px;
    }
    
    .event-button {
      display: inline-block;
      padding: 15px 40px;
      background-color: #aaff00;
      color: #000000;
      text-decoration: none;
      border-radius: 5px;
      font-weight: 700;
      font-size: 18px;
      text-transform: uppercase;
      letter-spacing: 1px;
      transition: all 0.3s ease;
      box-shadow: 0 0 20px rgba(170, 255, 0, 0.5);
    }
    
    .event-button:hover {
      transform: scale(1.05);
      box-shadow: 0 0 30px rgba(170, 255, 0, 0.7);
    }
  </style>
</head>
<body>
<div class=""countdown-container"">
  <div class=""bf-header"">
    <h1 class=""bf-title"">BLACK <span>FRIDAY</span></h1>
  </div>
  
  <div class=""days-remaining"">
    <strong>{{days}}</strong> DAYS TO GO
  </div>
  
  <div class=""countdown-timer"">
    <div class=""time-block"">
      <div class=""time-value"">{{days}}</div>
      <div class=""time-label"">Days</div>
    </div>
    <div class=""time-block"">
      <div class=""time-value"">{{hours}}</div>
      <div class=""time-label"">Hours</div>
    </div>
    <div class=""time-block"">
      <div class=""time-value"">{{minutes}}</div>
      <div class=""time-label"">Mins</div>
    </div>
    <div class=""time-block"">
      <div class=""time-value"">{{seconds}}</div>
      <div class=""time-label"">Secs</div>
    </div>
  </div>
  
  <div class=""discount-badge"">Up to 70% off</div>
  
  <p class=""event-description"">{{eventDescription}}</p>
  
  <a href=""{{eventLink}}"" class=""event-button"">{{buttonText}}</a>
</div>
</body>
</html>";
        }

        private static string GetLaunchingSoonTemplate()
        {
            return @"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <title>Launching Soon Countdown</title>
  <style>
    .countdown-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      height: 100%;
      width: 100%;
      padding: 30px;
      box-sizing: border-box;
      font-family: Raleway, sans-serif;
      background-color: #2c3e50;
      color: white;
      text-align: center;
      position: relative;
      overflow: hidden;
    }
    
    .bg-overlay {
      position: absolute;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background: linear-gradient(45deg, rgba(52, 73, 94, 0.8) 0%, rgba(44, 62, 80, 0.8) 100%);
      z-index: -1;
    }
    
    .launch-badge {
      background-color: #e74c3c;
      color: white;
      font-size: 14px;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 2px;
      padding: 8px 20px;
      border-radius: 30px;
      margin-bottom: 30px;
    }
    
    .event-title {
      font-size: 36px;
      font-weight: 300;
      margin: 0;
      margin-bottom: 10px;
      letter-spacing: 2px;
    }
    
    .event-subtitle {
      font-size: 16px;
      font-weight: 300;
      margin-bottom: 40px;
      opacity: 0.8;
      max-width: 600px;
    }
    
    .countdown-timer {
      display: flex;
      align-items: flex-start;
      justify-content: center;
      margin-bottom: 40px;
    }
    
    .time-block {
      background-color: rgba(255, 255, 255, 0.1);
      border-radius: 5px;
      padding: 15px 10px;
      margin: 0 5px;
      min-width: 70px;
      backdrop-filter: blur(5px);
    }
    
    .time-value {
      font-size: 36px;
      font-weight: 200;
      margin-bottom: 5px;
    }
    
    .time-label {
      font-size: 12px;
      text-transform: uppercase;
      letter-spacing: 1px;
      opacity: 0.7;
    }
    
    .notification-form {
      display: flex;
      flex-direction: column;
      align-items: center;
      margin-bottom: 30px;
      width: 100%;
      max-width: 500px;
    }
    
    .form-title {
      font-size: 14px;
      margin-bottom: 15px;
      font-weight: 600;
    }
    
    .email-input {
      width: 100%;
      padding: 12px 15px;
      border-radius: 5px;
      border: none;
      margin-bottom: 10px;
      font-family: Raleway, sans-serif;
      font-size: 14px;
    }
    
    .event-button {
      display: inline-block;
      padding: 12px 30px;
      background-color: #3498db;
      color: white;
      text-decoration: none;
      border-radius: 5px;
      font-weight: 600;
      transition: all 0.3s ease;
      text-transform: uppercase;
      letter-spacing: 1px;
      font-size: 12px;
      border: none;
      cursor: pointer;
      width: 100%;
    }
    
    .event-button:hover {
      background-color: #2980b9;
    }
    
    .social-links {
      display: flex;
      justify-content: center;
      margin-top: 20px;
    }
    
    .social-link {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      width: 40px;
      height: 40px;
      border-radius: 50%;
      background-color: rgba(255, 255, 255, 0.1);
      margin: 0 5px;
      color: white;
      text-decoration: none;
      font-size: 16px;
      transition: all 0.3s ease;
    }
    
    .social-link:hover {
      background-color: #3498db;
      transform: translateY(-3px);
    }
  </style>
</head>
<body>
<div class=""countdown-container"">
  <div class=""bg-overlay""></div>
  
  <div class=""launch-badge"">Launching Soon</div>
  
  <h1 class=""event-title"">{{eventName}}</h1>
  <p class=""event-subtitle"">{{eventDescription}}</p>
  
  <div class=""countdown-timer"">
    <div class=""time-block"">
      <div class=""time-value"">{{days}}</div>
      <div class=""time-label"">Days</div>
    </div>
    <div class=""time-block"">
      <div class=""time-value"">{{hours}}</div>
      <div class=""time-label"">Hours</div>
    </div>
    <div class=""time-block"">
      <div class=""time-value"">{{minutes}}</div>
      <div class=""time-label"">Minutes</div>
    </div>
    <div class=""time-block"">
      <div class=""time-value"">{{seconds}}</div>
      <div class=""time-label"">Seconds</div>
    </div>
  </div>
  
  <div class=""notification-form"">
    <div class=""form-title"">GET NOTIFIED WHEN WE LAUNCH</div>
    <input type=""email"" class=""email-input"" placeholder=""Your email address"">
    <button class=""event-button"">NOTIFY ME</button>
  </div>
  
  <a href=""{{eventLink}}"" class=""event-button"">{{buttonText}}</a>
  
  <div class=""social-links"">
    <a href=""#"" class=""social-link"">f</a>
    <a href=""#"" class=""social-link"">t</a>
    <a href=""#"" class=""social-link"">in</a>
    <a href=""#"" class=""social-link"">ig</a>
  </div>
</div>
</body>
</html>";
        
        }
    }
}