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

namespace FacebookTimerPosts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;

        public AuthController(IUserRepository userRepository, IConfiguration configuration, IUserSubscriptionRepository userSubscriptionRepository)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _userSubscriptionRepository = userSubscriptionRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            if (await _userRepository.UserExistsAsync(registerDto.Email))
            {
                return BadRequest("Email is already registered");
            }

            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                RegistrationDate = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userRepository.RegisterUserAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            var result = await _userRepository.LoginUserAsync(loginDto.Email, loginDto.Password);

            if (!result.Succeeded)
            {
                return Unauthorized("Invalid credentials");
            }

            await _userRepository.UpdateLastLoginDateAsync(user);

            var userToReturn = await GetUserDto(user);

            return Ok(new
            {
                token = GenerateJwtToken(user),
                user = userToReturn
            });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var userToReturn = await GetUserDto(user);

            return Ok(userToReturn);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _userRepository.LogoutAsync();
            return Ok(new { message = "Logged out successfully" });
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
        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalAuthDto externalAuth)
        {
            try
            {
                var result = await _userRepository.ExternalLoginAsync(externalAuth);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
