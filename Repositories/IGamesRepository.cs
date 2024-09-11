using GameStore.Entities;

namespace GameStore.Repositories;

public interface IGamesRepository
{
// Asynchronous methods must return a Task type   
    Task CreateAsync(Game game);
    Task DeleteAsync(int id);
    Task<Game?> GetAsync(int id);
    Task<IEnumerable<Game>> GetAllAsync(int pageNumber, int pageSize, string? filter);
    Task UpdateGameAsync(Game updatedGame);
    Task<int> CountAsync(string?filter);
}