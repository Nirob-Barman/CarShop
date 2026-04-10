using CarShop.Application.DTOs.Import;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Interfaces
{
    public interface IBulkImportService
    {
        Task<Result<BulkImportResultDto>> ImportCarsFromCsvAsync(Stream csvStream);
    }
}
