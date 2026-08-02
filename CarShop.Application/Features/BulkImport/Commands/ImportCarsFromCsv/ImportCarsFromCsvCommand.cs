using CarShop.Application.DTOs.Import;
using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.BulkImport.Commands.ImportCarsFromCsv
{
    public class ImportCarsFromCsvCommand : IRequest<Result<BulkImportResultDto>>
    {
        public Stream CsvStream { get; set; }

        public ImportCarsFromCsvCommand(Stream csvStream)
        {
            CsvStream = csvStream;
        }
    }
}
