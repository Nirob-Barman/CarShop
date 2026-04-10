namespace CarShop.Application.DTOs.Import
{
    public class BulkImportResultDto
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
