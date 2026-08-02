using CarShop.Application.DTOs.Import;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Wrappers;
using MediatR;
using BrandEntity = CarShop.Domain.Entities.Brand;
using CarEntity = CarShop.Domain.Entities.Car;

namespace CarShop.Application.Features.BulkImport.Commands.ImportCarsFromCsv
{
    public class ImportCarsFromCsvCommandHandler : IRequestHandler<ImportCarsFromCsvCommand, Result<BulkImportResultDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public ImportCarsFromCsvCommandHandler(IUnitOfWork unitOfWork, IAuditLogService auditLogService, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<BulkImportResultDto>> Handle(ImportCarsFromCsvCommand request, CancellationToken cancellationToken)
        {
            var result = new BulkImportResultDto();
            var carsToAdd = new List<CarEntity>();

            using var reader = new StreamReader(request.CsvStream);
            var header = await reader.ReadLineAsync(); // skip header

            int rowNumber = 1;
            while (!reader.EndOfStream)
            {
                rowNumber++;
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                if (parts.Length < 5)
                {
                    result.Errors.Add($"Row {rowNumber}: Invalid format. Expected: Title,Description,Price,Quantity,BrandName");
                    result.FailureCount++;
                    continue;
                }

                var title = parts[0].Trim();
                var description = parts[1].Trim();
                var priceStr = parts[2].Trim();
                var quantityStr = parts[3].Trim();
                var brandName = parts[4].Trim();

                if (string.IsNullOrEmpty(title))
                {
                    result.Errors.Add($"Row {rowNumber}: Title is required.");
                    result.FailureCount++;
                    continue;
                }

                if (!decimal.TryParse(priceStr, out var price) || price <= 0)
                {
                    result.Errors.Add($"Row {rowNumber}: Invalid price '{priceStr}'.");
                    result.FailureCount++;
                    continue;
                }

                if (!int.TryParse(quantityStr, out var quantity) || quantity < 0)
                {
                    result.Errors.Add($"Row {rowNumber}: Invalid quantity '{quantityStr}'.");
                    result.FailureCount++;
                    continue;
                }

                var brand = await _unitOfWork.Repository<BrandEntity>().FirstOrDefaultAsync(
                    b => b.Name != null && b.Name.ToLower() == brandName.ToLower());

                if (brand == null)
                {
                    result.Errors.Add($"Row {rowNumber}: Brand '{brandName}' not found.");
                    result.FailureCount++;
                    continue;
                }

                carsToAdd.Add(new CarEntity
                {
                    Title = title,
                    Description = description,
                    Price = price,
                    Quantity = quantity,
                    BrandId = brand.Id
                });

                result.SuccessCount++;
            }

            if (carsToAdd.Any())
            {
                await _unitOfWork.Repository<CarEntity>().AddRangeAsync(carsToAdd);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _auditLogService.LogAsync("Car", "BulkImport", _userContextService.UserId, _userContextService.Email,
                    $"Imported {result.SuccessCount} cars via CSV");
            }

            return Result<BulkImportResultDto>.Ok(result,
                $"Import complete. {result.SuccessCount} succeeded, {result.FailureCount} failed.");
        }
    }
}
