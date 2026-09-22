namespace CarShop.Application.DTOs.File
{
    public class FileUpload
    {
        public Stream? Content { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public long Size { get; set; }
    }
}
