using FiapCloudGames.Users.Business;
using FiapCloudGames.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Users.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            var token = await _authService.RegisterAsync(user);
            return Ok(new { Token = token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var token = await _authService.LoginAsync(email, password);
            return Ok(new { Token = token });
        }
    }
}
