using AutoMapper;
using FacebookTimerPosts.DTOs;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.SqlServer.Server;
using FacebookTimerPosts.Services.Repository;

namespace FacebookTimerPosts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IFileUploadService _fileUploadService;

        public AuthController(
            IUserRepository userRepository,
            IConfiguration configuration,
            IUserSubscriptionRepository userSubscriptionRepository,
            UserManager<User> userManager,
            IFileUploadService fileUploadService,
            SignInManager<User> signInManager)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _userSubscriptionRepository = userSubscriptionRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _fileUploadService = fileUploadService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            if (await _userRepository.UserExistsAsync(registerDto.Email))
            {
                return BadRequest(new { message = "Email is already registered" });
            }

            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                RegistrationDate = DateTime.UtcNow,
                IsActive = true,
                EmailConfirmed = true // Set to true for now, implement email verification later
            };

            var result = await _userRepository.RegisterUserAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Registration failed",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var result = await _userRepository.LoginUserAsync(loginDto.Email, loginDto.Password);

            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            await _userRepository.UpdateLastLoginDateAsync(user);

            var userToReturn = await GetUserDto(user);

            return Ok(new
            {
                token = GenerateJwtToken(user),
                user = userToReturn,
                expiresIn = 86400 // 24 hours
            });
        }

        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalAuthDto externalAuth)
        {
            try
            {
                if (externalAuth.Provider?.ToLower() != "google")
                {
                    return BadRequest(new { message = "Only Google authentication is currently supported" });
                }

                // Validate Google JWT token and extract payload
                var googlePayload = await ValidateGoogleTokenAsync(externalAuth.Credential);
                if (googlePayload == null)
                {
                    return BadRequest(new { message = "Invalid Google token" });
                }

                // Check if user exists
                var existingUser = await _userManager.FindByEmailAsync(googlePayload.Email);
                bool isNewUser = false;
                User user;

                if (existingUser == null)
                {
                    // Handle new user registration
                    if (externalAuth.Action?.ToLower() == "login")
                    {
                        return BadRequest(new { message = "No account found with this email. Please register first." });
                    }

                    // Create new user
                    user = new User
                    {
                        UserName = googlePayload.Email,
                        Email = googlePayload.Email,
                        FirstName = googlePayload.GivenName ?? "",
                        LastName = googlePayload.FamilyName ?? "",
                        RegistrationDate = DateTime.UtcNow,
                        IsActive = true,
                        EmailConfirmed = googlePayload.EmailVerified,
                        LastLoginDate = DateTime.UtcNow
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        return BadRequest(new
                        {
                            message = "Failed to create user account",
                            errors = createResult.Errors.Select(e => e.Description)
                        });
                    }

                    // Add external login info
                    await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", googlePayload.Sub, "Google"));
                    isNewUser = true;
                }
                else
                {
                    // Handle existing user login
                    if (externalAuth.Action?.ToLower() == "register")
                    {
                        return BadRequest(new { message = "An account with this email already exists. Please sign in instead." });
                    }

                    user = existingUser;

                    // Update last login date
                    user.LastLoginDate = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    // Check if Google login is already associated
                    var existingLogin = await _userManager.FindByLoginAsync("Google", googlePayload.Sub);
                    if (existingLogin == null)
                    {
                        // Add Google login to existing account
                        await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", googlePayload.Sub, "Google"));
                    }
                }

                // Generate JWT token
                var token = GenerateJwtToken(user);
                var userDto = await GetUserDto(user);

                var result = new ExternalAuthResultDto
                {
                    Token = token,
                    User = userDto,
                    ExpiresIn = 86400, // 24 hours
                    IsNewUser = isNewUser
                };

                return Ok(result);
            }
            catch (InvalidJwtException)
            {
                return BadRequest(new { message = "Invalid Google token format" });
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, new { message = "An error occurred during authentication", details = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var userToReturn = await GetUserDto(user);

            return Ok(userToReturn);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            // Implement refresh token logic here
            // This is a placeholder - you'll need to implement proper refresh token handling
            return BadRequest(new { message = "Refresh token functionality not implemented yet" });
        }

        private async Task<GoogleTokenPayload> ValidateGoogleTokenAsync(string credential)
        {
            try
            {
                var googleClientId = _configuration["GoogleAuth:ClientId"];

                // Validate the token with Google
                var payload = await GoogleJsonWebSignature.ValidateAsync(credential, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { googleClientId }
                });

                return new GoogleTokenPayload
                {
                    Sub = payload.Subject,
                    Email = payload.Email,
                    EmailVerified = payload.EmailVerified,
                    Name = payload.Name,
                    GivenName = payload.GivenName,
                    FamilyName = payload.FamilyName,
                    Picture = payload.Picture,
                    Hd = payload.HostedDomain,
                    Iat = payload.IssuedAtTimeSeconds ?? 0,
                    Exp = payload.ExpirationTimeSeconds ?? 0
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Google token validation failed: {ex.Message}");
                return null;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName ?? ""),
                new Claim(ClaimTypes.Surname, user.LastName ?? ""),
                new Claim("userId", user.Id)
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

        private async Task<UserDto> GetUserDto(User user)
        {
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

            return new UserDto
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
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var userDto = await MapToUserDto(user);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Update user properties
                user.FirstName = updateProfileDto.FirstName ?? user.FirstName;
                user.LastName = updateProfileDto.LastName ?? user.LastName;

                // Note: Email updates should be handled separately with verification
                if (!string.IsNullOrEmpty(updateProfileDto.PhoneNumber))
                {
                    user.PhoneNumber = updateProfileDto.PhoneNumber;
                }

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        message = "Failed to update profile",
                        errors = result.Errors.Select(e => e.Description)
                    });
                }

                var userDto = await MapToUserDto(user);
                return Ok(new { message = "Profile updated successfully", user = userDto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpGet("active-sessions")]
        public async Task<IActionResult> GetActiveSessions()
        {
            try
            {
                var userId = GetCurrentUserId();
                var activeSessions = await GetActiveSessionsAsync(userId);

                return Ok(activeSessions);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, new { message = "An error occurred while retrieving active sessions", details = ex.Message });
            }
        }
        [HttpDelete("sessions/{sessionId}")]
        public async Task<IActionResult> TerminateSession(string sessionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var currentSessionId = GetCurrentSessionId();

                // Don't allow terminating the current session
                if (sessionId == currentSessionId)
                {
                    return BadRequest(new { message = "Cannot terminate the current session" });
                }

                var success = await TerminateUserSessionAsync(userId, sessionId);

                if (success)
                {
                    // Log security event
                    await LogSecurityEventAsync(userId, "Session terminated", $"Session {sessionId} was terminated by user");
                    return Ok(new { message = "Session terminated successfully" });
                }

                return NotFound(new { message = "Session not found" });
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, new { message = "An error occurred while terminating the session", details = ex.Message });
            }
        }
        [HttpPost("logout-all-sessions")]
        public async Task<IActionResult> LogoutAllSessions()
        {
            try
            {
                var userId = GetCurrentUserId();
                var currentSessionId = GetCurrentSessionId();

                var terminatedCount = await TerminateAllUserSessionsExceptCurrentAsync(userId, currentSessionId);

                // Log security event
                await LogSecurityEventAsync(userId, "All sessions terminated", $"User terminated {terminatedCount} other sessions");

                return Ok(new { message = $"Successfully terminated {terminatedCount} sessions", count = terminatedCount });
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, new { message = "An error occurred while terminating sessions", details = ex.Message });
            }
        }
        [HttpGet("security-settings")]
        public async Task<IActionResult> GetSecuritySettings()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Get external login providers
                var externalLogins = await _userManager.GetLoginsAsync(user);
                var externalLoginDtos = externalLogins.Select(login => new UserLoginInfoDto
                {
                    LoginProvider = login.LoginProvider,
                    ProviderDisplayName = login.LoginProvider, // You might want to map this to a friendly name
                    ConnectedDate = DateTime.UtcNow // You might want to store this separately
                }).ToList();

                // Get last password change date
                var lastPasswordChange = await GetLastPasswordChangeAsync(userId);

                // Get notification preferences (you might need to implement this)
                var notificationPreferences = await GetNotificationPreferencesAsync(userId);

                var securitySettings = new SecuritySettingsDto
                {
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    EmailNotifications = notificationPreferences?.EmailNotifications ?? true,
                    SmsNotifications = notificationPreferences?.SmsNotifications ?? false,
                    LoginAlerts = notificationPreferences?.LoginAlerts ?? true,
                    LastPasswordChange = lastPasswordChange,
                    ExternalLogins = externalLoginDtos,
                    ActiveSessions = await GetActiveSessionsAsync(userId),
                    SecurityEvents = await GetRecentSecurityEventsAsync(userId)
                };

                return Ok(securitySettings);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, new { message = "An error occurred while retrieving security settings", details = ex.Message });
            }
        }
        private async Task<NotificationPreferencesDto?> GetNotificationPreferencesAsync(string userId)
        {
            // This would typically come from a user preferences table
            // For now, return default preferences
            return await Task.FromResult(new NotificationPreferencesDto
            {
                EmailNotifications = true,
                SmsNotifications = false,
                PushNotifications = true,
                LoginAlerts = true,
                SecurityAlerts = true,
                PostSuccessNotifications = true,
                PostFailureNotifications = true,
                SubscriptionExpiryNotifications = true,
                MarketingEmails = false,
                WeeklyReports = true
            });
        }
        [HttpGet("account-stats")]
        public async Task<IActionResult> GetAccountStats()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Get basic account stats
                var registrationDays = (int)(DateTime.UtcNow - user.RegistrationDate).TotalDays;
                var lastLoginDays = user.LastLoginDate.HasValue
                    ? (int)(DateTime.UtcNow - user.LastLoginDate.Value).TotalDays
                    : 0;

                // Get login count from audit logs (you might need to implement this)
                var loginCount = await GetUserLoginCountAsync(userId);

                // Calculate security score
                var securityScore = await CalculateSecurityScoreAsync(user);

                // Get data usage (total file uploads size)
                var dataUsage = await GetUserDataUsageAsync(userId);

                // Get last password change date
                var lastPasswordChange = await GetLastPasswordChangeAsync(userId);

                var accountStats = new AccountStatsDto
                {
                    LoginCount = loginCount,
                    SecurityScore = securityScore,
                    DataUsage = dataUsage,
                    LastPasswordChange = lastPasswordChange,
                    AccountAge = registrationDays,
                    LastLoginDays = lastLoginDays,
                    EmailVerified = user.EmailConfirmed,
                    PhoneVerified = user.PhoneNumberConfirmed,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    ProfileCompleteness = CalculateProfileCompleteness(user)
                };

                return Ok(accountStats);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, new { message = "An error occurred while retrieving account statistics", details = ex.Message });
            }
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        message = "Failed to change password",
                        errors = result.Errors.Select(e => e.Description)
                    });
                }

                return Ok(new { message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        //[HttpGet("stats")]
        //public async Task<IActionResult> GetUserStats()
        //{
        //    try
        //    {
        //        var userId = GetCurrentUserId();
        //        var stats = await _postRepository.GetUserStatsAsync(userId);

        //        return Ok(stats);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Internal server error" });
        //    }
        //}
        [HttpPut("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file uploaded" });
                }

                // Validate file type and size
                if (!IsValidImageFile(file))
                {
                    return BadRequest(new { message = "Invalid file type. Only images are allowed." });
                }

                if (file.Length > 5 * 1024 * 1024) // 5MB limit
                {
                    return BadRequest(new { message = "File size must be less than 5MB" });
                }

                var userId = GetCurrentUserId();
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Upload file and get URL
                var photoUrl = await _fileUploadService.UploadUserAvatarAsync(file, userId);

                // Update user's photo URL
                user.PhotoURL = photoUrl;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "Failed to update profile picture" });
                }

                return Ok(new { message = "Profile picture updated successfully", photoURL = photoUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        private bool IsValidImageFile(IFormFile file)
        {
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            return allowedTypes.Contains(file.ContentType.ToLower());
        }

        [HttpPost("update-email")]
        public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailDto updateEmailDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Verify current password
                var passwordValid = await _userManager.CheckPasswordAsync(user, updateEmailDto.Password);
                if (!passwordValid)
                {
                    return BadRequest(new { message = "Current password is incorrect" });
                }

                // Check if email is already taken
                var existingUser = await _userManager.FindByEmailAsync(updateEmailDto.NewEmail);
                if (existingUser != null && existingUser.Id != userId)
                {
                    return BadRequest(new { message = "Email is already registered" });
                }

                // Generate email change token
                var token = await _userManager.GenerateChangeEmailTokenAsync(user, updateEmailDto.NewEmail);

                // In a real application, you would send this token via email
                // For now, we'll return it in the response (not recommended for production)
                return Ok(new
                {
                    message = "Email verification token generated. Check your email to complete the change.",
                    token = token // Remove this in production
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpPost("confirm-email-change")]
        public async Task<IActionResult> ConfirmEmailChange([FromBody] ConfirmEmailChangeDto confirmDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var result = await _userManager.ChangeEmailAsync(user, confirmDto.NewEmail, confirmDto.Token);

                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        message = "Failed to change email",
                        errors = result.Errors.Select(e => e.Description)
                    });
                }

                // Update username to match new email
                await _userManager.SetUserNameAsync(user, confirmDto.NewEmail);

                return Ok(new { message = "Email changed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User not authenticated");
        }
        private string GetCurrentSessionId()
        {
            // Extract session ID from JWT token or generate based on user agent + IP
            var userAgent = Request.Headers["User-Agent"].ToString();
            var ipAddress = GetClientIpAddress();
            return GenerateSessionId(userAgent, ipAddress);
        }
        private string GetClientIpAddress()
        {
            var ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            return ipAddress ?? "Unknown";
        }
        private string GenerateSessionId(string userAgent, string ipAddress)
        {
            var combined = $"{userAgent}_{ipAddress}_{DateTime.UtcNow.Date}";
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(combined));
            return Convert.ToBase64String(hash).Substring(0, 16);
        }

        private async Task<int> GetUserLoginCountAsync(string userId)
        {
            // This would typically come from an audit log table
            // For now, return a mock value or implement based on your logging system
            return await Task.FromResult(new Random().Next(10, 100));
        }
        private async Task<int> CalculateSecurityScoreAsync(User user)
        {
            await Task.CompletedTask; // Placeholder for async operation

            int score = 0;
            int maxScore = 100;

            // Email verified (20 points)
            if (user.EmailConfirmed) score += 20;

            // Phone verified (15 points)
            if (user.PhoneNumberConfirmed) score += 15;

            // Two-factor enabled (25 points)
            if (user.TwoFactorEnabled) score += 25;

            // Profile completeness (20 points)
            var completeness = CalculateProfileCompleteness(user);
            score += (int)(completeness / 100.0 * 20);

            // Recent password change (10 points)
            var lastPasswordChange = await GetLastPasswordChangeAsync(user.Id);
            if (lastPasswordChange.HasValue && lastPasswordChange.Value > DateTime.UtcNow.AddDays(-90))
                score += 10;

            // Active account (10 points)
            if (user.IsActive) score += 10;

            return Math.Min(score, maxScore);
        }
        private int CalculateProfileCompleteness(User user)
        {
            int completedFields = 0;
            int totalFields = 8;

            if (!string.IsNullOrEmpty(user.FirstName)) completedFields++;
            if (!string.IsNullOrEmpty(user.LastName)) completedFields++;
            if (!string.IsNullOrEmpty(user.Email)) completedFields++;
            if (!string.IsNullOrEmpty(user.PhoneNumber)) completedFields++;
            if (!string.IsNullOrEmpty(user.PhotoURL)) completedFields++;
            if (!string.IsNullOrEmpty(user.Bio)) completedFields++;
            if (!string.IsNullOrEmpty(user.Website)) completedFields++;
            if (!string.IsNullOrEmpty(user.Location)) completedFields++;

            return (int)((double)completedFields / totalFields * 100);
        }
        private async Task<long> GetUserDataUsageAsync(string userId)
        {
            // This would typically calculate total file upload sizes
            // For now, return a mock value
            return await Task.FromResult(new Random().Next(1000000, 50000000)); // 1MB to 50MB
        }

        private async Task<DateTime?> GetLastPasswordChangeAsync(string userId)
        {
            // This would typically come from an audit log or user security table
            // For now, return a mock value
            return await Task.FromResult(DateTime.UtcNow.AddDays(-new Random().Next(1, 180)));
        }
        private async Task<UserDto> MapToUserDto(User user)
        {
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

            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                PhotoURL = user.PhotoURL,
                RegistrationDate = user.RegistrationDate,
                LastLoginDate = user.LastLoginDate,
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed,
                CurrentSubscription = subscriptionDto
            };
        }
        private async Task<List<UserSessionDto>> GetActiveSessionsAsync(string userId)
        {
            // This would typically come from a sessions table or cache
            // For now, return mock data including the current session
            var currentSessionId = GetCurrentSessionId();
            var userAgent = Request.Headers["User-Agent"].ToString();
            var ipAddress = GetClientIpAddress();

            var sessions = new List<UserSessionDto>
            {
                new UserSessionDto
                {
                    Id = currentSessionId,
                    DeviceInfo = ParseUserAgent(userAgent),
                    IpAddress = ipAddress,
                    Location = await GetLocationFromIpAsync(ipAddress),
                    LoginTime = DateTime.UtcNow.AddHours(-2),
                    LastActivity = DateTime.UtcNow,
                    IsCurrent = true
                }
            };

            // Add some mock additional sessions
            var random = new Random();
            for (int i = 0; i < random.Next(1, 4); i++)
            {
                sessions.Add(new UserSessionDto
                {
                    Id = Guid.NewGuid().ToString("N").Substring(0, 16),
                    DeviceInfo = GetRandomDeviceInfo(),
                    IpAddress = GetRandomIpAddress(),
                    Location = GetRandomLocation(),
                    LoginTime = DateTime.UtcNow.AddDays(-random.Next(1, 7)),
                    LastActivity = DateTime.UtcNow.AddHours(-random.Next(1, 24)),
                    IsCurrent = false
                });
            }

            return await Task.FromResult(sessions);
        }
        private async Task<bool> TerminateUserSessionAsync(string userId, string sessionId)
        {
            // This would typically remove the session from your session store
            // For now, return true to simulate success
            return await Task.FromResult(true);
        }

        private async Task<int> TerminateAllUserSessionsExceptCurrentAsync(string userId, string currentSessionId)
        {
            // This would typically remove all sessions except the current one
            // For now, return a mock count
            return await Task.FromResult(new Random().Next(1, 5));
        }

        private async Task LogSecurityEventAsync(string userId, string eventType, string description)
        {
            // This would typically log to a security audit table
            // For now, just log to console or your logging system
            Console.WriteLine($"Security Event - User: {userId}, Type: {eventType}, Description: {description}");
            await Task.CompletedTask;
        }

        private async Task<string> GetLocationFromIpAsync(string ipAddress)
        {
            // This would typically use a geolocation service
            // For now, return a mock location
            var locations = new[] { "Lahore, Pakistan", "Karachi, Pakistan", "Islamabad, Pakistan", "Unknown" };
            return await Task.FromResult(locations[new Random().Next(locations.Length)]);
        }

        private string ParseUserAgent(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "Unknown Device";

            if (userAgent.Contains("Chrome"))
                return "Chrome on " + (userAgent.Contains("Windows") ? "Windows" : userAgent.Contains("Mac") ? "macOS" : "Linux");
            if (userAgent.Contains("Firefox"))
                return "Firefox on " + (userAgent.Contains("Windows") ? "Windows" : userAgent.Contains("Mac") ? "macOS" : "Linux");
            if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome"))
                return "Safari on " + (userAgent.Contains("iPhone") ? "iPhone" : "macOS");
            if (userAgent.Contains("Edge"))
                return "Edge on Windows";

            return "Unknown Browser";
        }

        private string GetRandomDeviceInfo()
        {
            var devices = new[]
            {
                "Chrome on Windows",
                "Firefox on macOS",
                "Safari on iPhone",
                "Chrome on Android",
                "Edge on Windows",
                "Chrome on macOS"
            };
            return devices[new Random().Next(devices.Length)];
        }

        private string GetRandomIpAddress()
        {
            var random = new Random();
            return $"{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}";
        }

        private string GetRandomLocation()
        {
            var locations = new[]
            {
                "Lahore, Pakistan",
                "Karachi, Pakistan",
                "Islamabad, Pakistan",
                "Faisalabad, Pakistan",
                "Rawalpindi, Pakistan"
            };
            return locations[new Random().Next(locations.Length)];
        }
        private async Task<List<SecurityEventDto>> GetRecentSecurityEventsAsync(string userId)
        {
            // This would typically come from a security audit log table
            // For now, return mock data
            var events = new List<SecurityEventDto>
            {
                new SecurityEventDto
                {
                    Id = Guid.NewGuid().ToString(),
                    EventType = "Login",
                    Description = "Successful login",
                    IpAddress = GetClientIpAddress(),
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    Timestamp = DateTime.UtcNow.AddHours(-1),
                    IsSuccessful = true
                },
                new SecurityEventDto
                {
                    Id = Guid.NewGuid().ToString(),
                    EventType = "Password Change",
                    Description = "Password changed successfully",
                    IpAddress = GetClientIpAddress(),
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    Timestamp = DateTime.UtcNow.AddDays(-5),
                    IsSuccessful = true
                }
            };

            return await Task.FromResult(events);
        }

    }
  
    // Additional DTO for refresh token
    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; }
    }
}