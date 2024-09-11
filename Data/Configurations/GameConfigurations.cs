using GameStore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Data.Configurations;

public class GameConfigurations : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.Property(game => game.Price).HasPrecision(5, 2);
        // Alternate way of creating constraints instead of creating in the Game entity class
        // builder.Property(game => game.Name).HasMaxLength(50);
    }
}