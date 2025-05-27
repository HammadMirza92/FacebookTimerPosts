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

        public AuthController(
            IUserRepository userRepository,
            IConfiguration configuration,
            IUserSubscriptionRepository userSubscriptionRepository,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _userSubscriptionRepository = userSubscriptionRepository;
            _userManager = userManager;
            _signInManager = signInManager;
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
    }

    // Additional DTO for refresh token
    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; }
    }
}