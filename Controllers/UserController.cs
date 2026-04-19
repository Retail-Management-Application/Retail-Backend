using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailOrdering.API.DTOs.User;
using RetailOrdering.API.Services;

namespace RetailOrdering.API.Controllers;

[ApiController]
[Route("api/users")]
[AllowAnonymous]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // GET api/user
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    // GET api/user/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    // POST api/user
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = await _userService.CreateUserAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.UserId }, created);
    }

    // PUT api/user/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _userService.UpdateUserAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    // DELETE api/user/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _userService.DeleteUserAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}