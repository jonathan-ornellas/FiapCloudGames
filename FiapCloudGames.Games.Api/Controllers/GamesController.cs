namespace FiapCloudGames.Games.Api.Controllers;

using FiapCloudGames.Games.Api.DTOs;
using FiapCloudGames.Games.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    [Authorize]
    public async Task<ActionResult<GameResponse>> CreateGame([FromBody] CreateGameRequest request)
    {
        var response = await _gameService.CreateGameAsync(request);
        return CreatedAtAction(nameof(GetGameById), new { id = response.Id }, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameResponse>> GetGameById(Guid id)
    {
        var game = await _gameService.GetGameByIdAsync(id);
        if (game == null)
            return NotFound();

        return Ok(game);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameResponse>>> GetAllGames()
    {
        var games = await _gameService.GetAllGamesAsync();
        return Ok(games);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<GameResponse>>> SearchGames([FromQuery] string query)
    {
        var results = await _gameService.SearchGamesAsync(query);
        return Ok(results);
    }

    [HttpGet("recommendations/{userId}")]
    [Authorize]
    public async Task<ActionResult<RecommendationResponse>> GetRecommendations(Guid userId)
    {
        var recommendations = await _gameService.GetRecommendationsAsync(userId);
        return Ok(recommendations);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<GameResponse>> UpdateGame(Guid id, [FromBody] UpdateGameRequest request)
    {
        try
        {
            var response = await _gameService.UpdateGameAsync(id, request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteGame(Guid id)
    {
        var result = await _gameService.DeleteGameAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
