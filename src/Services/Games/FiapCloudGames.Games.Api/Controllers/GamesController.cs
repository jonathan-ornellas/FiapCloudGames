using Microsoft.AspNetCore.Mvc;
using FiapCloudGames.Games.Api.DTOs;
using FiapCloudGames.Games.Business;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using FiapCloudGames.Games.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FiapCloudGames.Games.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly GamesContext _context;

    public GamesController(IGameService gameService, GamesContext context)
    {
        _gameService = gameService;
        _context = context;
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
        var game = new Game(
            createGameDto.Title, 
            createGameDto.Description, 
            createGameDto.Category, 
            new Money(createGameDto.Price), 
            createGameDto.Rating
        );
        await _gameService.CreateAsync(game);
        return Ok();
    }

    [HttpPost("purchase")]
    [Authorize]
    public async Task<IActionResult> Purchase(PurchaseGameDto purchaseDto)
    {
        var game = await _context.Games.FindAsync(purchaseDto.GameId);
        if (game == null)
            return NotFound("Game not found");

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var existingPurchase = await _context.UserLibraries
            .FirstOrDefaultAsync(ul => ul.UserId == userId && ul.GameId == purchaseDto.GameId);

        if (existingPurchase != null)
            return BadRequest("Game already purchased");

        var userLibrary = new UserLibrary(userId, purchaseDto.GameId, new Money(purchaseDto.Amount));
        _context.UserLibraries.Add(userLibrary);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Game purchased successfully", LibraryId = userLibrary.Id });
    }

    [HttpGet("library")]
    [Authorize]
    public async Task<IActionResult> GetLibrary()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var library = await _context.UserLibraries
            .Where(ul => ul.UserId == userId)
            .Join(_context.Games,
                ul => ul.GameId,
                g => g.Id,
                (ul, g) => new
                {
                    g.Id,
                    g.Title,
                    g.Description,
                    g.Genre,
                    g.Rating,
                    PurchaseDate = ul.PurchaseDate,
                    PurchasePrice = ul.PurchasePrice.Value
                })
            .ToListAsync();

        return Ok(library);
    }

    [HttpGet("recommendations")]
    [Authorize]
    public async Task<IActionResult> GetRecommendations([FromQuery] int limit = 10)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var purchasedGameIds = await _context.UserLibraries
            .Where(ul => ul.UserId == userId)
            .Select(ul => ul.GameId)
            .ToListAsync();

        var recommendations = await _context.Games
            .Where(g => !purchasedGameIds.Contains(g.Id))
            .OrderByDescending(g => g.Rating)
            .Take(limit)
            .ToListAsync();

        return Ok(recommendations);
    }
}
