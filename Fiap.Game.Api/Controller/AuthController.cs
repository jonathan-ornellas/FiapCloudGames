using AutoMapper;
using Fiap.Game.Api.Contracts.Request;
using Fiap.Game.Api.Contracts.Response;
using Fiap.Game.Domain.Entities;
using Fiap.Game.Domain.Interface.Service;
using Fiap.Game.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Fiap.Game.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly IMapper _mapper;
        private readonly IPasswordHasherService _passwordHasher;

        public AuthController(IAuthService auth, IMapper mapper, IPasswordHasherService passwordHasher)
        {
            _auth = auth;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req)
        {
            var password = new Password(req.Password);
            var hashedPassword = _passwordHasher.Hash(password.Value);
            
            var user = new User(req.Name, new Email(req.Email), hashedPassword);
            var token = await _auth.RegisterAsync(user);
            return Ok(new AuthResponse(token));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
        {
            var token = await _auth.LoginAsync(req.Email, req.Password);
            return Ok(new AuthResponse(token));
        }

        [HttpGet("me")]
        [Authorize]
        public ActionResult<object> Me()
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.Claims.FirstOrDefault(c => c.Type.Contains("email"))?.Value;
            var name = User.FindFirstValue(ClaimTypes.Name);
            var role = User.FindFirstValue(ClaimTypes.Role);
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Ok(new { id = sub, name, email, role });
        }
    }
}
