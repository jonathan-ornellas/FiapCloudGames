namespace FiapCloudGames.Games.Api.Repositories;

using FiapCloudGames.Games.Api.Data;
using FiapCloudGames.Games.Api.Models;
using Microsoft.EntityFrameworkCore;

public interface IGameRepository
{
    Task<Game?> GetByIdAsync(Guid id);
    Task<Game?> GetByTitleAsync(string title);
    Task<Game> CreateAsync(Game game);
    Task<Game> UpdateAsync(Game game);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Game>> GetAllAsync();
    Task<IEnumerable<Game>> GetByGenreAsync(string genre);
}

public class GameRepository : IGameRepository
{
    private readonly GamesContext _context;

    public GameRepository(GamesContext context)
    {
        _context = context;
    }

    public async Task<Game?> GetByIdAsync(Guid id)
    {
        return await _context.Games.FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<Game?> GetByTitleAsync(string title)
    {
        return await _context.Games.FirstOrDefaultAsync(g => g.Title == title);
    }

    public async Task<Game> CreateAsync(Game game)
    {
        game.Id = Guid.NewGuid();
        game.CreatedAt = DateTime.UtcNow;
        game.UpdatedAt = DateTime.UtcNow;

        _context.Games.Add(game);
        await _context.SaveChangesAsync();
        return game;
    }

    public async Task<Game> UpdateAsync(Game game)
    {
        game.UpdatedAt = DateTime.UtcNow;
        _context.Games.Update(game);
        await _context.SaveChangesAsync();
        return game;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var game = await GetByIdAsync(id);
        if (game == null) return false;

        _context.Games.Remove(game);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        return await _context.Games.ToListAsync();
    }

    public async Task<IEnumerable<Game>> GetByGenreAsync(string genre)
    {
        return await _context.Games
            .Where(g => g.Genre == genre)
            .ToListAsync();
    }
}
