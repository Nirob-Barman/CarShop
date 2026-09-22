using CarShop.Application.DTOs.Import;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.BulkImport.Commands.ImportCarsFromCsv
{
    public record ImportCarsFromCsvCommand(Stream CsvStream) : IRequest<Result<BulkImportResultDto>>;
}
