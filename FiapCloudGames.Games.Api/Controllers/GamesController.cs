using FiapCloudGames.Games.Business;
using FiapCloudGames.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Games.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Game game)
        {
            await _gameService.CreateAsync(game);
            return Ok();
        }
    }
}
