using AutoMapper;
using FacebookTimerPosts.DTOs;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FacebookTimerPosts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public AuthController(
            IUserRepository userRepository,
            IMapper mapper,
            IConfiguration config)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await _userRepository.CheckUserExistsAsync(registerDto.Username))
                return BadRequest("Username is already taken");

            if (await _userRepository.CheckEmailExistsAsync(registerDto.Email))
                return BadRequest("Email is already registered");

            var user = await _userRepository.RegisterAsync(
                registerDto.Username,
                registerDto.Email,
                registerDto.Password);

            var userDto = _mapper.Map<UserDto>(user);

            return new JsonResult(new
            {
                token = GenerateJwtToken(user),
                user = userDto
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userRepository.AuthenticateAsync(
                loginDto.Username,
                loginDto.Password);

            if (user == null) return Unauthorized("Invalid username or password");

            user.LastActive = DateTime.UtcNow;
            await _userRepository.SaveAllAsync();

            var userDto = _mapper.Map<UserDto>(user);

            return new JsonResult(new
            {
                token = GenerateJwtToken(user),
                user = userDto
            });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config["TokenKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
