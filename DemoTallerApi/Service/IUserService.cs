using DemoTallerApi.DTOs;

namespace DemoTallerApi.Service
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<PaginatedResponseDto<UserDto>> GetAllUsersAsync(int pageNumber, int pageSize);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto> UpdateUserAsync(int id, CreateUserDto updateUserDto);
        Task DeleteUserAsync(int id);
        Task<bool> UserExistsAsync(string email);
    }
}
