using Azure.Storage.Blobs;
using GameStore.Authorization;
using GameStore.Cors;
using GameStore.Data;
using GameStore.Endpoints;
using GameStore.ErrorHandling;
using GameStore.ImageUpload;
using GameStore.Middleware;
using GameStore.OpenAPI;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;


var builder = WebApplication.CreateBuilder(args);

// The essential builder services of the SQL database service
builder.AddRepositories();

// Log each request
builder.Services.AddHttpLogging(o => {});


// Authorization
builder.Services.AddAuthentication().AddJwtBearer().AddJwtBearer("Auth0"); 
builder.Services.AddGameStoreAuthorization();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new(1.0);
    options.AssumeDefaultVersionWhenUnspecified = true;
})
.AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");
 
builder.Services.AddGameStoreCors(builder.Configuration);

builder.Services.AddSwaggerGen()
    .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>()
    .AddEndpointsApiExplorer();

builder.Services.AddSingleton<IImageUploader>(
    new ImageUploader(
    new BlobContainerClient(
        builder.Configuration.GetConnectionString("AzureStorage"),
        "images"
        )
    )
);

builder.Services.AddAntiforgery();
builder.Logging.AddAzureWebAppDiagnostics();
// Services must be before app
var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.ConfigureExceptionHandler());

app.UseMiddleware<RequestTimingMiddleware>();

// Initialize Logger
app.Logger.LogInformation(5, "The database has been initialized.");

app.UseHttpLogging();

// Routing
app.MapGamesEndpoints();
app.MapImagesEndpoints();

app.UseCors();

app.UseSwagger();

app.UseGameStoreSwagger();
app.UseAntiforgery();

app.Run();