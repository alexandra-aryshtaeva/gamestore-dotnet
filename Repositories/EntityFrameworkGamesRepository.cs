using GameStore.Data;
using GameStore.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Repositories;

public class EntityFrameworkGamesRepository : IGamesRepository
{
    private readonly GameStoreContext DbContext;
    private readonly ILogger<EntityFrameworkGamesRepository> _logger;
    
    public EntityFrameworkGamesRepository
        (GameStoreContext dbContext, ILogger<EntityFrameworkGamesRepository> logger)
    {
        DbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<Game>> GetAllAsync(int pageNumber, int pageSize, string?filter)
    {
        var skipCount = (pageSize * (pageNumber - 1));
        
        // AsNoTracking speedups process since returning a list of games is a simple process 
        return await FilterGames(filter)
            .OrderBy(g => g.Id)
            .Skip(skipCount)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }
    
    public async Task<Game?> GetAsync(int id)
    {
        return await DbContext.Games.FindAsync(id);
    }
    
    public async Task CreateAsync(Game game)
    {
        // AddAsync needs to wait for the data to check the generated ID   
        await DbContext.Games.AddAsync(game);
        await DbContext.SaveChangesAsync();
        
        // Logger
        _logger.LogInformation("Created game with name:{Name}, price:{Price}.", game.Name, game.Price);
    }

    public async Task UpdateGameAsync(Game updatedGame)
    {
        // Updated doesn't need to wait for the ID
        DbContext.Games.Update(updatedGame);
        await DbContext.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        await DbContext.Games.Where(g => g.Id == id).ExecuteDeleteAsync();
    }
    
    public async Task<int> CountAsync(string? filter)
    {
     return await FilterGames(filter).CountAsync();
    }

    private IQueryable<Game> FilterGames(string? filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return DbContext.Games;
        }
        
        return DbContext.Games.Where(g => g.Name.Contains(filter) || g.Genre.Contains(filter));
    }
}