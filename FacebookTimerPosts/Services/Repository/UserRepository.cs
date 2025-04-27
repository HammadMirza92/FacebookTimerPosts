using FacebookTimerPosts.AppDbContext;
using FacebookTimerPosts.DTOs;
using FacebookTimerPosts.Enums;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using FacebookTimerPosts.Services.Repository.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FacebookTimerPosts.Services.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;

        public UserRepository(UserManager<User> userManager, SignInManager<User> signInManager, IHttpClientFactory httpClientFactory, IUserSubscriptionRepository userSubscriptionRepository, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpClientFactory = httpClientFactory;
            _userSubscriptionRepository = userSubscriptionRepository;
            _configuration = configuration;
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<IList<string>> GetUserRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<SignInResult> LoginUserAsync(string email, string password)
        {
            return await _signInManager.PasswordSignInAsync(email, password, false, false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> RegisterUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task UpdateLastLoginDateAsync(User user)
        {
            user.LastLoginDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }
        public async Task<User> FindUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<IdentityResult> CreateUserAsync(User user)
        {
            return await _userManager.CreateAsync(user);
        }
        public async Task<ExternalAuthResponseDto> ExternalLoginAsync(ExternalAuthDto externalAuth)
        {
            try
            {
                // Validate token with the provider
                var validatedInfo = await ValidateExternalToken(externalAuth);
                if (validatedInfo == null)
                {
                    throw new UnauthorizedAccessException("Invalid external authentication token");
                }

                // Check if user exists
                var user = await _userManager.FindByEmailAsync(validatedInfo.Email);
                bool isNewUser = false;

                if (user == null)
                {
                    // Create new user
                    user = new User
                    {
                        UserName = validatedInfo.Email,
                        Email = validatedInfo.Email,
                        FirstName = validatedInfo.FirstName,
                        LastName = validatedInfo.LastName,
                        RegistrationDate = DateTime.UtcNow,
                        IsActive = true
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create user: {string.Join(", ", result.Errors)}");
                    }

                    // Add external login
                    result = await _userManager.AddLoginAsync(user, new UserLoginInfo(
                        externalAuth.Provider,
                        validatedInfo.Id,
                        externalAuth.Provider));

                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to add external login: {string.Join(", ", result.Errors)}");
                    }

                    isNewUser = true;
                }

                // Update last login date
                await UpdateLastLoginDateAsync(user);

                // Generate JWT token
                var token = GenerateJwtToken(user);

                // Get user subscription
                var currentSubscription = await _userSubscriptionRepository.GetCurrentSubscriptionAsync(user.Id);
                UserSubscriptionDto subscriptionDto = null;

                if (currentSubscription != null)
                {
                    int daysRemaining = (int)(currentSubscription.EndDate - DateTime.UtcNow).TotalDays;

                    subscriptionDto = new UserSubscriptionDto
                    {
                        Id = currentSubscription.Id,
                        SubscriptionPlanId = currentSubscription.SubscriptionPlanId,
                        SubscriptionPlanName = currentSubscription.SubscriptionPlan.Name,
                        StartDate = currentSubscription.StartDate,
                        EndDate = currentSubscription.EndDate,
                        IsActive = currentSubscription.IsActive,
                        AutoRenew = currentSubscription.AutoRenew,
                        DaysRemaining = daysRemaining > 0 ? daysRemaining : 0
                    };
                }

                // Create user DTO
                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    RegistrationDate = user.RegistrationDate,
                    LastLoginDate = user.LastLoginDate,
                    IsActive = user.IsActive,
                    CurrentSubscription = subscriptionDto
                };

                return new ExternalAuthResponseDto
                {
                    Token = token,
                    User = userDto,
                    IsNewUser = isNewUser
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"External login failed: {ex.Message}");
            }
        }

        private async Task<ExternalUserInfo> ValidateExternalToken(ExternalAuthDto externalAuth)
        {
            if (externalAuth.Provider == "Google")
            {
                return await ValidateGoogleToken(externalAuth.IdToken);
            }
            else if (externalAuth.Provider == "Facebook")
            {
                return await ValidateFacebookToken(externalAuth.IdToken);
            }
            else
            {
                throw new NotSupportedException($"Provider {externalAuth.Provider} is not supported");
            }
        }

        private async Task<ExternalUserInfo> ValidateGoogleToken(string token)
        {
            var client = _httpClientFactory.CreateClient();

            // Google's token validation endpoint
            var response = await client.GetAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={token}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var payload = JsonConvert.DeserializeObject<GoogleTokenPayload>(content);

            // Verify token's audience matches your Google Client ID
            string clientId = _configuration["Authentication:Google:ClientId"];
            if (payload.aud != clientId)
            {
                return null;
            }

            return new ExternalUserInfo
            {
                Id = payload.sub,
                Email = payload.email,
                FirstName = payload.given_name,
                LastName = payload.family_name
            };
        }

        private async Task<ExternalUserInfo> ValidateFacebookToken(string token)
        {
            var client = _httpClientFactory.CreateClient();

            // Facebook Graph API to validate token and get user info
            var response = await client.GetAsync($"https://graph.facebook.com/me?fields=id,email,first_name,last_name&access_token={token}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var payload = JsonConvert.DeserializeObject<FacebookUserInfo>(content);

            return new ExternalUserInfo
            {
                Id = payload.id,
                Email = payload.email,
                FirstName = payload.first_name,
                LastName = payload.last_name
            };
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = creds,
                Issuer = _configuration.GetSection("AppSettings:Issuer").Value,
                Audience = _configuration.GetSection("AppSettings:Audience").Value
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }

    // Helper classes for token validation
    public class ExternalUserInfo
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class GoogleTokenPayload
    {
        public string iss { get; set; }
        public string azp { get; set; }
        public string aud { get; set; }
        public string sub { get; set; }
        public string email { get; set; }
        public string email_verified { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string picture { get; set; }
        public string locale { get; set; }
        public long iat { get; set; }
        public long exp { get; set; }
    }

    public class FacebookUserInfo
    {
        public string id { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }

}

