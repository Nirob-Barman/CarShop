using System.Text.Json;
using CarShop.Application.DTOs.Car;
using CarShop.Application.DTOs.File;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.FileStorage;
using CarShop.Application.Interfaces.Persistence;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;
using CarShop.Domain.Entities;

namespace CarShop.Application.Services
{
    public class CarService : ICarService
    {
        private readonly IFileStorage _fileStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContextService _userContextService;

        public CarService(IUnitOfWork unitOfWork, IFileStorage fileStorage, IAuditLogService auditLogService, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _auditLogService = auditLogService;
            _userContextService = userContextService;
        }

        public async Task<Result<IEnumerable<CarDto>>> GetAllCarsAsync()
        {
            var cars = await _unitOfWork.Repository<Car>().GetAllAsync();
            var result = cars.Select(CarMapper.ToDto);
            return Result<IEnumerable<CarDto>>.Ok(result);
        }

        public async Task<Result<IEnumerable<CarDto>>> GetCarsByBrandIdAsync(int brandId)
        {
            var cars = await _unitOfWork.Repository<Car>().GetAllWithIncludesAsync(
                predicate: c => c.BrandId == brandId,
                selector: c => c,
                c => c.Brand!
            );
            var result = cars.Select(CarMapper.ToDto);
            return Result<IEnumerable<CarDto>>.Ok(result);
        }

        public async Task<Result<CarDto>> GetCarByIdAsync(int id)
        {
            var car = await _unitOfWork.Repository<Car>().GetByIdAsync(id);
            if (car == null)
                return Result<CarDto>.Fail("Car not found");

            return Result<CarDto>.Ok(CarMapper.ToDto(car));
        }

        public async Task<Result<int>> CreateCarAsync(CarDto dto, FileUploadDto? file)
        {
            if (file != null)
            {
                dto.ImageUrl = await _fileStorage.UploadFileAsync(file.Content!, file.FileName!, "uploads/car");
            }

            var car = CarMapper.ToEntity(dto);
            await _unitOfWork.Repository<Car>().AddAsync(car);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("Car", "Create", _userContextService.UserId, _userContextService.Email,
                $"Created car: {car.Title} (Id: {car.Id})",
                entityId: car.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                newValues: JsonSerializer.Serialize(CarMapper.ToDto(car)));

            return Result<int>.Ok(car.Id, "Car created successfully.");
        }

        public async Task<Result<string>> UpdateCarAsync(int id, CarDto dto, FileUploadDto? file)
        {
            var car = await _unitOfWork.Repository<Car>().GetByIdAsync(id);
            if (car == null)
                return Result<string>.Fail("Car not found.");

            var oldValues = JsonSerializer.Serialize(CarMapper.ToDto(car));

            if (file != null)
            {
                if (!string.IsNullOrEmpty(car.ImageUrl))
                    await _fileStorage.DeleteFileAsync(car.ImageUrl);

                dto.ImageUrl = await _fileStorage.UploadFileAsync(file.Content!, file.FileName!, "uploads/car");
            }
            else
            {
                // No new image uploaded — keep the existing one
                dto.ImageUrl = car.ImageUrl;
            }

            CarMapper.UpdateEntity(car, dto);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("Car", "Update", _userContextService.UserId, _userContextService.Email,
                $"Updated car: {car.Title} (Id: {car.Id})",
                entityId: car.Id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: oldValues,
                newValues: JsonSerializer.Serialize(CarMapper.ToDto(car)));

            return Result<string>.Ok(null, "Car updated successfully.");
        }

        public async Task<Result<string>> DeleteCarAsync(int id)
        {
            var car = await _unitOfWork.Repository<Car>().GetByIdAsync(id);
            if (car == null)
                return Result<string>.Fail("Car not found.");

            if (!string.IsNullOrEmpty(car.ImageUrl))
                await _fileStorage.DeleteFileAsync(car.ImageUrl);

            var oldValues = JsonSerializer.Serialize(CarMapper.ToDto(car));

            _unitOfWork.Repository<Car>().Remove(car);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("Car", "Delete", _userContextService.UserId, _userContextService.Email,
                $"Deleted car: {car.Title} (Id: {id})",
                entityId: id,
                ipAddress: _userContextService.IpAddress,
                userAgent: _userContextService.UserAgent,
                oldValues: oldValues);

            return Result<string>.Ok(null, "Car deleted successfully.");
        }

        public async Task<Result<PagedResult<CarDto>>> SearchCarsAsync(CarSearchDto searchDto)
        {
            var cars = await _unitOfWork.Repository<Car>().GetAllWithIncludesAsync(
                predicate: c =>
                    (string.IsNullOrEmpty(searchDto.Keyword) ||
                        (c.Title != null && c.Title.ToLower().Contains(searchDto.Keyword.ToLower())) ||
                        (c.Description != null && c.Description.ToLower().Contains(searchDto.Keyword.ToLower()))) &&
                    (string.IsNullOrEmpty(searchDto.BrandName) ||
                        (c.Brand != null && c.Brand.Name != null && c.Brand.Name.ToLower() == searchDto.BrandName.ToLower())) &&
                    (!searchDto.MinPrice.HasValue || c.Price >= searchDto.MinPrice.Value) &&
                    (!searchDto.MaxPrice.HasValue || c.Price <= searchDto.MaxPrice.Value),
                selector: c => c,
                c => c.Brand!
            );

            var carList = cars.ToList();

            // Sorting
            carList = searchDto.SortBy?.ToLower() switch
            {
                "price_asc" => carList.OrderBy(c => c.Price).ToList(),
                "price_desc" => carList.OrderByDescending(c => c.Price).ToList(),
                "title" => carList.OrderBy(c => c.Title).ToList(),
                _ => carList.OrderByDescending(c => c.Id).ToList() // "newest" default
            };

            var totalCount = carList.Count;
            var page = searchDto.Page < 1 ? 1 : searchDto.Page;
            var pageSize = searchDto.PageSize < 1 ? 10 : searchDto.PageSize;

            var pagedItems = carList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(CarMapper.ToDto)
                .ToList();

            return Result<PagedResult<CarDto>>.Ok(new PagedResult<CarDto>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        public async Task<Result<IEnumerable<CarDto>>> GetCarsByIdsAsync(IEnumerable<int> ids)
        {
            var idList = ids.ToList();
            var cars = await _unitOfWork.Repository<Car>().GetAllWithIncludesAsync(
                predicate: c => idList.Contains(c.Id),
                selector: c => c,
                c => c.Brand!
            );

            var result = cars.Select(CarMapper.ToDto);
            return Result<IEnumerable<CarDto>>.Ok(result);
        }

        public async Task<Result<IEnumerable<CarDto>>> GetTopRatedCarsAsync(int count = 4)
        {
            // Load all comments with ratings, group by car, compute average
            var comments = await _unitOfWork.Repository<Comment>().GetAllAsync(
                c => c.Rating.HasValue,
                c => new { c.CarId, c.Rating }
            );

            var topCarIds = comments
                .GroupBy(c => c.CarId)
                .Select(g => new { CarId = g.Key, AvgRating = g.Average(c => c.Rating!.Value) })
                .OrderByDescending(x => x.AvgRating)
                .Take(count)
                .Select(x => x.CarId)
                .ToList();

            if (!topCarIds.Any())
            {
                // Fall back to newest cars when no ratings exist yet
                var newest = await _unitOfWork.Repository<Car>().GetAllWithIncludesAsync(
                    c => c.Quantity > 0, c => c, c => c.Brand!);
                return Result<IEnumerable<CarDto>>.Ok(
                    newest.OrderByDescending(c => c.Id).Take(count).Select(CarMapper.ToDto));
            }

            var cars = await _unitOfWork.Repository<Car>().GetAllWithIncludesAsync(
                predicate: c => topCarIds.Contains(c.Id),
                selector: c => c,
                c => c.Brand!
            );

            return Result<IEnumerable<CarDto>>.Ok(cars.Select(CarMapper.ToDto));
        }

        public async Task<Result<IEnumerable<CarDto>>> GetRecentCarsAsync(int count = 4)
        {
            var cars = await _unitOfWork.Repository<Car>().GetAllWithIncludesAsync(
                c => c, c => c.Brand!);

            var result = cars.OrderByDescending(c => c.Id).Take(count).Select(CarMapper.ToDto);
            return Result<IEnumerable<CarDto>>.Ok(result);
        }
    }
}
