using AutoMapper;
using DemoTallerApi.Data;
using DemoTallerApi.DTOs;
using DemoTallerApi.Helpers;
using DemoTallerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoTallerApi.Service
{
    public class UserService : IUserService
    {
        private readonly StoreDBContext _context;
        private readonly IMapper _mapper;

        public UserService(StoreDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Where(u => u.IsActive)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users
                .Where(u => u.IsActive)
                .FirstOrDefaultAsync(u => u.Email == email);

            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<PaginatedResponseDto<UserDto>> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _context.Users.Where(u => u.IsActive).CountAsync();
            
            var users = await _context.Users
                .Where(u => u.IsActive)
                .OrderByDescending(u => u.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userDtos = _mapper.Map<List<UserDto>>(users);

            return new PaginatedResponseDto<UserDto>
            {
                Items = userDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            if (await UserExistsAsync(createUserDto.Email))
            {
                throw new InvalidOperationException("Ya existe un usuario con este email");
            }

            var user = _mapper.Map<UserEntity>(createUserDto);
            user.PasswordHash = PasswordHelper.HashPassword(createUserDto.Password);
            user.CreatedDate = DateTime.Now;
            user.IsActive = true;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> UpdateUserAsync(int id, CreateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || !user.IsActive)
            {
                throw new KeyNotFoundException("Usuario no encontrado");
            }

            // Check if email is being changed and if it already exists
            if (user.Email != updateUserDto.Email && await UserExistsAsync(updateUserDto.Email))
            {
                throw new InvalidOperationException("Ya existe un usuario con este email");
            }

            user.Email = updateUserDto.Email;
            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.Role = updateUserDto.Role;
            user.UpdatedDate = DateTime.Now;

            if (!string.IsNullOrEmpty(updateUserDto.Password))
            {
                user.PasswordHash = PasswordHelper.HashPassword(updateUserDto.Password);
            }

            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado");
            }

            // Soft delete
            user.IsActive = false;
            user.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email && u.IsActive);
        }
    }
}
