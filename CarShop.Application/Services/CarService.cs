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

        public CarService(IUnitOfWork unitOfWork, IFileStorage fileStorage)
        {
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
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
                selector: c => c,   // select full Car entity
                c => c.Brand!   // include the Brand navigation property
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

            return Result<int>.Ok(car.Id, "Car created successfully.");
        }

        public async Task<Result<string>> UpdateCarAsync(int id, CarDto dto, FileUploadDto? file)
        {
            var car = await _unitOfWork.Repository<Car>().GetByIdAsync(id);
            if (car == null)
                return Result<string>.Fail("Car not found.");

            if (file != null)
            {
                if (!string.IsNullOrEmpty(car.ImageUrl))
                    await _fileStorage.DeleteFileAsync(car.ImageUrl);

                dto.ImageUrl = await _fileStorage.UploadFileAsync(file.Content!, file.FileName!, "uploads/car");
            }

            CarMapper.UpdateEntity(car, dto);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok(null, "Car updated successfully.");
        }

        public async Task<Result<string>> DeleteCarAsync(int id)
        {
            var car = await _unitOfWork.Repository<Car>().GetByIdAsync(id);
            if (car == null)
                return Result<string>.Fail("Car not found.");

            if (!string.IsNullOrEmpty(car.ImageUrl))
                await _fileStorage.DeleteFileAsync(car.ImageUrl);


            _unitOfWork.Repository<Car>().Remove(car);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Ok(null, "Car deleted successfully.");
        }
    }
}