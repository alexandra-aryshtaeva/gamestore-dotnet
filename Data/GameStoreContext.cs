using System.Reflection;
using GameStore.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data;

public class GameStoreContext : DbContext
{
    public GameStoreContext(DbContextOptions<GameStoreContext> options)
        : base(options)
    {
    }

    public DbSet<Game> Games => Set<Game>();

    // Override the base class from DbContext to apply the configuration of the migration
    // (ex: the price column precision value) 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Applies every single configuration of the project
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}