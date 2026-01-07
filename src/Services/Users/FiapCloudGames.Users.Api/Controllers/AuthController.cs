using Microsoft.AspNetCore.Mvc;
using FiapCloudGames.Users.Api.DTOs;
using FiapCloudGames.Users.Business.Services;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.ValueObjects;

namespace FiapCloudGames.Users.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
    {
        var user = new User(registerUserDto.Name, new Email(registerUserDto.Email), registerUserDto.Password);
        var token = await _authService.RegisterAsync(user);
        return Ok(new { Token = token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDto loginUserDto)
    {
        var token = await _authService.LoginAsync(new Email(loginUserDto.Email), loginUserDto.Password);
        return Ok(new { Token = token });
    }
}
