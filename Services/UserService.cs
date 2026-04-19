using RetailOrdering.API.DTOs.User;
using RetailOrdering.API.Models;
using RetailOrdering.API.Repositories;

namespace RetailOrdering.API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(ToResponseDto);
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user is null ? null : ToResponseDto(user);
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
    {
        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            Role = dto.Role,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var created = await _userRepository.CreateAsync(user);
        return ToResponseDto(created);
    }

    public async Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null) return null;

        // GetByIdAsync uses AsNoTracking, so we need a tracked copy
        var tracked = new User
        {
            UserId = user.UserId,
            FullName = dto.FullName,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            IsActive = dto.IsActive
        };

        var updated = await _userRepository.UpdateAsync(tracked);
        return ToResponseDto(updated);
    }

    public async Task<bool> DeleteUserAsync(int id)
        => await _userRepository.DeleteAsync(id);

    // ── Mapper ────────────────────────────────────────────────────────────────
    private static UserResponseDto ToResponseDto(User u) => new()
    {
        UserId = u.UserId,
        FullName = u.FullName,
        Email = u.Email,
        PhoneNumber = u.PhoneNumber,
        Address = u.Address,
        Role = u.Role,
        CreatedAt = u.CreatedAt,
        IsActive = u.IsActive
    };
}