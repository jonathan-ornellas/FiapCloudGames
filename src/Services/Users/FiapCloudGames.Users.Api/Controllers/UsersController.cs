using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FiapCloudGames.Users.Api.DTOs;
using FiapCloudGames.Users.Business;
using System.Security.Claims;

namespace FiapCloudGames.Users.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return NotFound();

        var profile = new UserProfileDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email.Value,
            Role = user.Role
        };

        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto updateProfileDto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return NotFound();

        user.Name = updateProfileDto.Name;
        await _userRepository.UpdateAsync(user);

        return NoContent();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userRepository.GetAllAsync();
        var userDtos = users.Select(u => new UserProfileDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email.Value,
            Role = u.Role
        });

        return Ok(userDtos);
    }

    [HttpPut("{id}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] string role)
    {
        if (role != "User" && role != "Admin")
            return BadRequest("Invalid role. Must be 'User' or 'Admin'.");

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        user.Role = role;
        await _userRepository.UpdateAsync(user);

        return NoContent();
    }
}
