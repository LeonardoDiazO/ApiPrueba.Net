using DemoTallerApi.DTOs;
using DemoTallerApi.Helpers;
using Microsoft.EntityFrameworkCore;
using DemoTallerApi.Data;

namespace DemoTallerApi.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly JwtHelper _jwtHelper;
        private readonly JwtSettings _jwtSettings;
        private readonly StoreDBContext _context;

        public AuthService(IUserService userService, JwtHelper jwtHelper, JwtSettings jwtSettings, StoreDBContext context)
        {
            _userService = userService;
            _jwtHelper = jwtHelper;
            _jwtSettings = jwtSettings;
            _context = context;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await ValidateUserAsync(loginDto.Email, loginDto.Password);
            
            if (user == null)
            {
                throw new UnauthorizedAccessException("Email o contraseña incorrectos");
            }

            var token = _jwtHelper.GenerateToken(user.Id, user.Email, user.Role);
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

            return new LoginResponseDto
            {
                Token = token,
                Expiration = expiration,
                User = user
            };
        }

        public async Task<LoginResponseDto> RegisterAsync(CreateUserDto createUserDto)
        {
            // Set default role if not specified
            if (string.IsNullOrEmpty(createUserDto.Role))
            {
                createUserDto.Role = "User";
            }

            var user = await _userService.CreateUserAsync(createUserDto);

            var token = _jwtHelper.GenerateToken(user.Id, user.Email, user.Role);
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

            return new LoginResponseDto
            {
                Token = token,
                Expiration = expiration,
                User = user
            };
        }

        public async Task<UserDto?> ValidateUserAsync(string email, string password)
        {
            var user = await _context.Users
                .Where(u => u.Email == email && u.IsActive)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            if (!PasswordHelper.VerifyPassword(password, user.PasswordHash))
            {
                return null;
            }

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                CreatedDate = user.CreatedDate,
                IsActive = user.IsActive
            };
        }
    }
}
