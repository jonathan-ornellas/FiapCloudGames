using Microsoft.AspNetCore.Mvc;
using FiapCloudGames.Games.Api.DTOs;
using FiapCloudGames.Games.Business;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;

namespace FiapCloudGames.Games.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GamesController : ControllerBase
{
    private readonly GameService _gameService;

    public GamesController(GameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var games = await _gameService.GetAllAsync();
        return Ok(games);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateGameDto createGameDto)
    {
        var game = new Game(createGameDto.Title, createGameDto.Description, createGameDto.Category, new Money(createGameDto.Price), createGameDto.Rating);
        await _gameService.CreateAsync(game);
        return Ok();
    }
}
