using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrdering.API.Common;
using RetailOrdering.API.DTOs.Auth;
using RetailOrdering.API.Repositories.Interfaces;

namespace RetailOrdering.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>GET /api/users — Admin: list all users</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            var dtos = users.Select(u => new UserProfileDto
            {
                Id = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Address = u.Address,
                Role = u.Role.ToString(),
                CreatedAt = u.CreatedAt
            });
            return Ok(ApiResponse<IEnumerable<UserProfileDto>>.Ok(dtos));
        }

        /// <summary>GET /api/users/{id} — Admin: get user by id</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse<object>.Error("User not found."));

            var dto = new UserProfileDto
            {
                Id = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };
            return Ok(ApiResponse<UserProfileDto>.Ok(dto));
        }

        /// <summary>DELETE /api/users/{id} — Admin: delete user</summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<object>.Error("User not found."));

            return Ok(ApiResponse<object>.Ok(null, "User deleted successfully."));
        }
    }
}