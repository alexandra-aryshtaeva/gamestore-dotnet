using GameStore.Entities;

namespace GameStore.Repositories;

//Methods implemented by the GamesRepository

public class InMemGamesRepository : IGamesRepository
{
    private readonly List<Game> games = new()
    {
        new Game()
        {
            Id = 1,
            Name = "Street Fighter II",
            Genre = " Figthing",
            Price = 19.99M,
            ReleaseDate = new DateTime(1991,2,1),
            ImgUrl = "https://placehold.co/100"
        },

        new Game()
        {
            Id = 2,
            Name = "Final Fantasy XIV",
            Genre = "Roleplaying",
            Price = 59.99M,
            ReleaseDate = new DateTime(2010,9,30),
            ImgUrl = "https://placehold.co/100"
        },

        new Game()
        {
            Id = 3,
            Name = "FIFA 23",
            Genre = "Sports",
            Price = 69.99M,
            ReleaseDate = new DateTime(2022,9,27),
            ImgUrl = "https://placehold.co/100"
        }
    };

    public async Task<IEnumerable<Game>> GetAllAsync(int pageNumber, int pageSize, string? filter)
    { 
        var skipCount = (pageSize * (pageNumber - 1));
        
        // FromResult():
        // nothing to await cuz there is no database so just return     
        return await Task.FromResult(games
            .Skip(skipCount)
            .Take(pageSize));
    }

    public async Task<Game?> GetAsync(int id)
    {
        return await Task.FromResult(games.Find(g => g.Id == id)) ;
    }

    public async Task CreateAsync(Game game)
    {
        
       game.Id = games.Max(game => game.Id) + 1;
       games.Add(game);
       //When the original return type is void just wait for the task to end 
       await Task.CompletedTask;
    }

    public async Task UpdateGameAsync(Game updatedGame)
    {
        var index = games.FindIndex(game => game.Id == updatedGame.Id);
        games[index] = updatedGame;
        await Task.CompletedTask;
    }

    public async Task<int> CountAsync(string? filter)
    {
        return await Task.FromResult(FilterGames(filter).Count());
    }

    public async Task DeleteAsync(int id)
    {
        var index = games.FindIndex(game => game.Id == id);
        games.RemoveAt(index);
        await Task.CompletedTask;
    }
    
    private IEnumerable<Game> FilterGames(string? filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return games;
        }
        
        return games.Where(g => g.Name.Contains(filter) || g.Genre.Contains(filter));
    }
}

