using FacebookTimerPosts.Enums;
using FacebookTimerPosts.Models;
using Microsoft.EntityFrameworkCore;

namespace FacebookTimerPosts.AppDbContext
{
    public static class Seed
    {
        public static async System.Threading.Tasks.Task SeedDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Apply pending migrations
                await dbContext.Database.MigrateAsync();

                // Seed data
                await SeedTemplates(dbContext);
            }
        }
        public static async Task SeedTemplates(ApplicationDbContext context)
        {
            if (await context.Templates.AnyAsync()) return;

            var templates = new List<Template>
        {
            // Free templates
            new Template
            {
                Name = "Basic Event",
                Description = "Simple countdown for general events",
                PreviewImageUrl = "/assets/templates/basic-event.jpg",
                BackgroundImageUrl = "/assets/backgrounds/event-bg.jpg",
                FontFamily = "Anton",
                PrimaryColor = "#FFFFFF",
                HasOverlay = true,
                Category = TemplateCategory.Event,
                MinimumSubscription = SubscriptionType.Free
            },
            new Template
            {
                Name = "Product Launch",
                Description = "Excitement for upcoming product launch",
                PreviewImageUrl = "/assets/templates/product-launch.jpg",
                BackgroundImageUrl = "/assets/backgrounds/product-bg.jpg",
                FontFamily = "Montserrat",
                PrimaryColor = "#F8F8F8",
                HasOverlay = true,
                Category = TemplateCategory.Launch,
                MinimumSubscription = SubscriptionType.Free
            },
            new Template
            {
                Name = "Sale Countdown",
                Description = "Create urgency for upcoming sales",
                PreviewImageUrl = "/assets/templates/sale.jpg",
                BackgroundImageUrl = "/assets/backgrounds/sale-bg.jpg",
                FontFamily = "Roboto",
                PrimaryColor = "#FFFF00",
                HasOverlay = false,
                Category = TemplateCategory.Sale,
                MinimumSubscription = SubscriptionType.Free
            },
            new Template
            {
                Name = "Holiday Special",
                Description = "Countdown to holiday events",
                PreviewImageUrl = "/assets/templates/holiday.jpg",
                BackgroundImageUrl = "/assets/backgrounds/holiday-bg.jpg",
                FontFamily = "Pacifico",
                PrimaryColor = "#FF0000",
                HasOverlay = true,
                Category = TemplateCategory.Holiday,
                MinimumSubscription = SubscriptionType.Free
            },
            new Template
            {
                Name = "Grand Opening",
                Description = "Build excitement for new location",
                PreviewImageUrl = "/assets/templates/opening.jpg",
                BackgroundImageUrl = "/assets/backgrounds/opening-bg.jpg",
                FontFamily = "Oswald",
                PrimaryColor = "#FFFFFF",
                HasOverlay = true,
                Category = TemplateCategory.Announcement,
                MinimumSubscription = SubscriptionType.Free
            },
            new Template
            {
                Name = "Contest End",
                Description = "Countdown to contest deadline",
                PreviewImageUrl = "/assets/templates/contest.jpg",
                BackgroundImageUrl = "/assets/backgrounds/contest-bg.jpg",
                FontFamily = "Roboto",
                PrimaryColor = "#00FFFF",
                HasOverlay = true,
                Category = TemplateCategory.Giveaway,
                MinimumSubscription = SubscriptionType.Free
            },
            new Template
            {
                Name = "Minimalist",
                Description = "Clean, simple countdown design",
                PreviewImageUrl = "/assets/templates/minimalist.jpg",
                BackgroundImageUrl = "/assets/backgrounds/minimal-bg.jpg",
                FontFamily = "Montserrat",
                PrimaryColor = "#000000",
                HasOverlay = false,
                Category = TemplateCategory.Other,
                MinimumSubscription = SubscriptionType.Free
            },
            
            // Standard plan templates (13 more)
            new Template
            {
                Name = "Premium Event",
                Description = "Elegant countdown for special events",
                PreviewImageUrl = "/assets/templates/premium-event.jpg",
                BackgroundImageUrl = "/assets/backgrounds/premium-event-bg.jpg",
                FontFamily = "Playfair Display",
                PrimaryColor = "#FFFFFF",
                HasOverlay = true,
                Category = TemplateCategory.Event,
                MinimumSubscription = SubscriptionType.Standard
            },
            
            // Add more Standard templates here
            
            // Premium templates (more exclusive designs)
            new Template
            {
                Name = "Elite Launch",
                Description = "High-end product launch countdown",
                PreviewImageUrl = "/assets/templates/elite-launch.jpg",
                BackgroundImageUrl = "/assets/backgrounds/elite-launch-bg.jpg",
                FontFamily = "Playfair Display",
                PrimaryColor = "#FFFFFF",
                HasOverlay = true,
                Category = TemplateCategory.Launch,
                MinimumSubscription = SubscriptionType.Premium
            }
            
            // Add more Premium templates here
        };

            await context.Templates.AddRangeAsync(templates);
            await context.SaveChangesAsync();
        }
    }
}
