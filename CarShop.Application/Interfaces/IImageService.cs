
namespace CarShop.Application.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType);
    }
}
