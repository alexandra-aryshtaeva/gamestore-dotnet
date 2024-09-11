namespace GameStore.ImageUpload;

public interface IImageUploader
{
    Task<string> UploadImageAsync(IFormFile file);
}