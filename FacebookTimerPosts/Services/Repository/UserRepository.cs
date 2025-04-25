using FacebookTimerPosts.AppDbContext;
using FacebookTimerPosts.Enums;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using FacebookTimerPosts.Services.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace FacebookTimerPosts.Services.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .SingleOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetUserWithPagesAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.LinkedPages)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> GetUserWithPostsAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Posts)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<int> GetUserRemainingPostsForTodayAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return 0;

            var dailyLimit = user.SubscriptionType switch
            {
                SubscriptionType.Free => 3,
                SubscriptionType.Standard => 10,
                SubscriptionType.Premium => 20,
                _ => 0
            };

            var postsToday = await _context.Posts
                .CountAsync(p => p.UserId == userId &&
                            p.CreatedAt.Date == DateTime.UtcNow.Date);

            return Math.Max(0, dailyLimit - postsToday);
        }

        public async Task<bool> UpdateUserSubscriptionAsync(int userId, SubscriptionType type, DateTime endDate)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return false;

            user.SubscriptionType = type;
            user.SubscriptionEndDate = endDate;

            return await SaveAllAsync();
        }

        public async Task<bool> CheckUserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Username == username);

            if (user == null) return null;

            // Verify password hash
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public async Task<User> RegisterAsync(string username, string email, string password)
        {
            CreatePasswordHash(password, out string passwordHash, out string passwordSalt);

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            using var hmac = new HMACSHA512(Convert.FromBase64String(storedSalt));
            var computedHash = Convert.ToBase64String(
                hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));

            return computedHash == storedHash;
        }

        private void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = Convert.ToBase64String(hmac.Key);
            passwordHash = Convert.ToBase64String(
                hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
    }
}
