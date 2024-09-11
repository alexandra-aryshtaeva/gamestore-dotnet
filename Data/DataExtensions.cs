using GameStore.Repositories;
using Microsoft.EntityFrameworkCore;
namespace GameStore.Data;

public static class DataExtensions
{
    // Initialize Migrations when app starts (also async)
    public static async Task InitializeDbAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        //retrieve the dbContext
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
        // apply Migrate()    
        await dbContext.Database.MigrateAsync();
    }

    // Data To Program.cs file 
    public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
    {
        var connString = builder.Configuration.GetConnectionString("GameStoreContext");
        builder.Services.AddSqlServer<GameStoreContext>(connString);

        // Now the application is using the sql server, don't forget to change the parameters in here!!!
        builder.Services.AddScoped<IGamesRepository, EntityFrameworkGamesRepository>();
        return builder;
    }
}