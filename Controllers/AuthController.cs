using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailOrdering.API.Common;
using RetailOrdering.API.DTOs.Auth;
using RetailOrdering.API.Services.Interfaces;
using System.Security.Claims;

namespace RetailOrdering.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>POST /api/auth/register — Register a new customer</summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Validation failed.", ModelState));

            var result = await _authService.RegisterAsync(dto);
            return Ok(ApiResponse<AuthResponseDto>.Success(result, "Registration successful."));
        }

        /// <summary>POST /api/auth/login — Login and receive JWT</summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Validation failed.", ModelState));

            var result = await _authService.LoginAsync(dto);
            return Ok(ApiResponse<AuthResponseDto>.Success(result, "Login successful."));
        }

        /// <summary>GET /api/auth/profile — Get current user's profile</summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();
            var result = await _authService.GetProfileAsync(userId);
            return Ok(ApiResponse<UserProfileDto>.Success(result));
        }

        /// <summary>PUT /api/auth/profile — Update current user's profile</summary>
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _authService.UpdateProfileAsync(userId, dto);
            return Ok(ApiResponse<UserProfileDto>.Success(result, "Profile updated successfully."));
        }

        // ── Private helper ────────────────────────────────────────────────
        private int GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User not authenticated.");
            return int.Parse(claim);
        }
    }
}
