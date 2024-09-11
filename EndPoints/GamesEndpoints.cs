using System.Diagnostics;
using GameStore.DTO_s;
using GameStore.Entities;
using GameStore.Repositories;
using GameStore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GameStore.Endpoints;

// Equivalent of Controllers

// Async Repositories (await before repositories variable)
public static class GamesEndpoints
{
    const string GetGameEndpointNameV1 = "GetGameV1";
    const string GetGameEndpointNameV2 = "GetGameV2";
    public static RouteGroupBuilder MapGamesEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.NewVersionedApi()
            .MapGroup("/games")
            .HasApiVersion(1.0)
            .HasApiVersion(2.0) 
            .WithParameterValidation()
            .WithOpenApi()
            .WithTags("Games");

// GET ALL V1       
        group.MapGet("/",
            async (IGamesRepository repository, 
                ILoggerFactory loggerFactory,
                [AsParameters]GetGamesDtoV1 request,
                HttpContext http) =>
            {
                var totalCount = await repository.CountAsync(request.Filter);
                http.Response.AddPaginationHeader(totalCount, request.PageSize);
                
                return Results.Ok((await repository.GetAllAsync(request.PageNumber, request.PageSize, request.Filter))
                    .Select(game => game.AsDtoV1()));
            }).MapToApiVersion(1.0)
            .WithSummary("Retrieve all Games")
            .WithDescription("Gets all available games and allows filtering and pagination");
// GET ID V1       
        group.MapGet("/{id}", async Task<Results<Ok<GameDtoV1>, NotFound>> (int id, IGamesRepository repository) =>
            {
                Game? game = await repository.GetAsync(id);
                return game is not null ? TypedResults.Ok(game.AsDtoV1()) : TypedResults.NotFound();
            }).WithName(GetGameEndpointNameV1).RequireAuthorization(Policies.ReadAccess).MapToApiVersion(1.0)
            .WithSummary("Gets game by Id")
            .WithDescription("Gets the game that has the specified Id");
        
// GET ALL V2       
        group.MapGet("/",
            async (IGamesRepository repository,
                ILoggerFactory loggerFactory, 
                [AsParameters]GetGamesDtoV2 request,
                HttpContext http) =>
            {
                var totalCount = await repository.CountAsync(request.Filter);
                http.Response.AddPaginationHeader(totalCount, request.PageSize);
                
                return Results.Ok((await repository.GetAllAsync(request.PageNumber, request.PageSize, request.Filter))
                    .Select(game => game.AsDtoV2()));
            }).MapToApiVersion(2.0)
            .WithSummary("Retrieve all Games")
            .WithDescription("Gets all available games and allows filtering and pagination");;

// GET ID V2       
        group.MapGet("/{id}", async Task<Results<Ok<GameDtoV2>, NotFound>> (int id, IGamesRepository repository) =>
            {
                Game? game = await repository.GetAsync(id);
                return game is not null ? TypedResults.Ok(game.AsDtoV2()) : TypedResults.NotFound();
            }).WithName(GetGameEndpointNameV2).RequireAuthorization(Policies.ReadAccess).MapToApiVersion(2.0)
            .WithSummary("Gets game by Id")
            .WithDescription("Gets the game that has the specified Id");

// CREATE GAME        
        group.MapPost("/", async Task<CreatedAtRoute<GameDtoV1>> (CreateGameDto gameDto, IGamesRepository repository) =>
            {
                Game game = new()
                {
                    Name = gameDto.Name,
                    Genre = gameDto.Genre,
                    Price = gameDto.Price,
                    ReleaseDate = gameDto.ReleaseDate,
                    ImgUrl = gameDto.ImageUri,
                };

                await repository.CreateAsync(game);
                return TypedResults.CreatedAtRoute(game.AsDtoV1(), GetGameEndpointNameV1, new { id = game.Id });
            }).RequireAuthorization(Policies.WriteAccess).MapToApiVersion(1.0)
            .WithSummary("Creates a new game")
            .WithDescription("Creates a new game with the specified properties");
            

// UPDATE GAME        
        group.MapPut("/{id}", async Task<Results<NoContent,NotFound>> (int id, UpdateGameDto updatedGame, IGamesRepository repository) =>
            {
                Game? existingGame = await repository.GetAsync(id);
                if (existingGame is null)
                {
                    return TypedResults.NotFound();
                }
 
                existingGame.Name = updatedGame.Name;
                existingGame.Genre = updatedGame.Genre;
                existingGame.Price = updatedGame.Price;
                existingGame.ReleaseDate = updatedGame.ReleaseDate;
                existingGame.ImgUrl = updatedGame.ImageUri;

                await repository.UpdateGameAsync(existingGame);
                return TypedResults.NoContent();
            }).RequireAuthorization(Policies.WriteAccess).MapToApiVersion(1.0)
        .WithSummary("Updates a game")
        .WithDescription("Updates all game properties with the specified Id");

// DELETE GAME
        group.MapDelete("/{id}", async (int id, IGamesRepository repository) =>
            {            
                Game? game = await repository.GetAsync(id);
                if (game is not null)
                {
                    await repository.DeleteAsync(id);
                }

                return TypedResults.NoContent();
            }).MapToApiVersion(1.0)
            .WithSummary("Deletes a game")
            .WithDescription("Deletes a game that has the specified Id");

        return group;
    }
}