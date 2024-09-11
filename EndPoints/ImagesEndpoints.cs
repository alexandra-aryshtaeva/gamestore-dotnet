using System.Net.Mime;
using GameStore.Authorization;
using GameStore.DTO_s;
using GameStore.ImageUpload;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GameStore.Endpoints;

public static class ImagesEndpoints
{
     public static RouteHandlerBuilder MapImagesEndpoints(this IEndpointRouteBuilder routes)
     {
          var api = routes.NewVersionedApi();
          
          return api.MapPost(
               "/images",
               async Task<Results<Ok<ImageUploadDto>, BadRequest>> (IFormFile file, IImageUploader imageUploader) =>
               {
                    if (file.Length <= 0)
                    {
                         return TypedResults.BadRequest();
                    }
                    
                    var imageUri = await imageUploader.UploadImageAsync(file);
                    
                    return TypedResults.Ok(new ImageUploadDto(imageUri));
               })
               .HasApiVersion(1.0)
               .MapToApiVersion(1.0)
               .WithOpenApi()
               .WithSummary("Uploads files to storage")
               .WithDescription("Uploads a file to storage and returns the url of the uploaded file")
               .WithTags("Images")
               .DisableAntiforgery()
               .RequireAuthorization(Policies.WriteAccess);



     }
         
}