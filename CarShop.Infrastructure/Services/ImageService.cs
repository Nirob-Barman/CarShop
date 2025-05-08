using System.Net.Http.Headers;
using System.Text.Json;

namespace CarShop.Infrastructure.Services
{
    public class ImageService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ImageService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType)
        {
            var client = _httpClientFactory.CreateClient();
            var apiKey = "20aa093c37c39290d0671f320463b0b2";

            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(imageStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            content.Add(fileContent, "image", fileName);

            var response = await client.PostAsync($"https://api.imgbb.com/1/upload?key={apiKey}", content);
            response.EnsureSuccessStatusCode(); // Throws if not 2xx

            var result = await response.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(result);
            var root = jsonDoc.RootElement;

            // Get data.url
            var imageUrl = root.GetProperty("data").GetProperty("url").GetString();

            return imageUrl!;
        }
    }
}
