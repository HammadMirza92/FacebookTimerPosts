using FacebookTimerPosts.Models;
using Microsoft.EntityFrameworkCore;

namespace FacebookTimerPosts.AppDbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<FacebookPage> FacebookPages { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<User>()
                .HasMany(u => u.LinkedPages)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Post>()
            .HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FacebookPage>()
                .HasMany(p => p.Posts)
                .WithOne(p => p.FacebookPage)
                .HasForeignKey(p => p.FacebookPageId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Template>()
                .HasMany(t => t.Posts)
                .WithOne(p => p.Template)
                .HasForeignKey(p => p.TemplateId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
