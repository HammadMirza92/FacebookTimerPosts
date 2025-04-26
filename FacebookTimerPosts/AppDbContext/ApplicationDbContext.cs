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

            builder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

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
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Post
            builder.Entity<Post>()
                .Property(p => p.Status)
                .HasConversion<string>();

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
                    Id = 1,
                    Name = "Basic Countdown",
                    Description = "A simple, clean countdown template",
                    DefaultFontFamily = "Arial",
                    PrimaryColor = "#000000",
                    RequiresSubscription = false,
                    IsActive = true,
                    CreatedAt = System.DateTime.UtcNow
                },
                new Template
                {
                    Id = 2,
                    Name = "Event Promotion",
                    Description = "Perfect for promoting upcoming events",
                    DefaultFontFamily = "Verdana",
                    PrimaryColor = "#FF5733",
                    RequiresSubscription = false,
                    IsActive = true,
                    CreatedAt = System.DateTime.UtcNow
                },
                new Template
                {
                    Id = 3,
                    Name = "Product Launch",
                    Description = "Build anticipation for product launches",
                    DefaultFontFamily = "Roboto",
                    PrimaryColor = "#3498DB",
                    RequiresSubscription = true,
                    MinimumSubscriptionPlanId = 2,
                    IsActive = true,
                    CreatedAt = System.DateTime.UtcNow
                }
            );
        }
    }
}
