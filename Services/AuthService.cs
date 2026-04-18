using RetailOrdering.API.Common.Enums;
using RetailOrdering.API.DTOs.Auth;
using RetailOrdering.API.Helpers;
using RetailOrdering.API.Models;
using RetailOrdering.API.Repositories.Interfaces;
using RetailOrdering.API.Services.Interfaces;

namespace RetailOrdering.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;

        public AuthService(IUserRepository userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(dto.Email))
                throw new InvalidOperationException("Email is already registered.");

            var user = new User
            {
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim().ToLower(),
                PasswordHash = PasswordHelper.HashPassword(dto.Password),
                PhoneNumber = dto.PhoneNumber?.Trim(),
                Address = dto.Address?.Trim(),
                Role = UserRole.Customer
            };

            var created = await _userRepository.CreateAsync(user);
            var token = _jwtHelper.GenerateToken(created);

            return BuildAuthResponse(created, token);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email)
                ?? throw new UnauthorizedAccessException("Invalid email or password.");

            if (!PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            var token = _jwtHelper.GenerateToken(user);
            return BuildAuthResponse(user, token);
        }

        public async Task<UserProfileDto> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            return MapToProfileDto(user);
        }

        public async Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found.");

            // Handle password change
            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
                    throw new InvalidOperationException("Current password is required to set a new password.");

                if (!PasswordHelper.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                    throw new UnauthorizedAccessException("Current password is incorrect.");

                user.PasswordHash = PasswordHelper.HashPassword(dto.NewPassword);
            }

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName.Trim();

            if (dto.PhoneNumber != null)
                user.PhoneNumber = dto.PhoneNumber.Trim();

            if (dto.Address != null)
                user.Address = dto.Address.Trim();

            var updated = await _userRepository.UpdateAsync(user);
            return MapToProfileDto(updated);
        }

        // ── Private helpers ──────────────────────────────────────────────
        private static AuthResponseDto BuildAuthResponse(User user, (string token, DateTime expiresAt) tokenData)
        {
            return new AuthResponseDto
            {
                Token = tokenData.token,
                ExpiresAt = tokenData.expiresAt,
                User = MapToProfileDto(user)
            };
        }

        private static UserProfileDto MapToProfileDto(User user) => new()
        {
            Id = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt
        };
    }
}
