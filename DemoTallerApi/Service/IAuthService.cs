using DemoTallerApi.DTOs;

namespace DemoTallerApi.Service
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<LoginResponseDto> RegisterAsync(CreateUserDto createUserDto);
        Task<UserDto?> ValidateUserAsync(string email, string password);
    }
}
