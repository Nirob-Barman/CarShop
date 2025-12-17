using CarShop.Application.DTOs.Car;
using CarShop.Application.DTOs.File;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.FileStorage;
using CarShop.Application.Interfaces.Repositories;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Services
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;
        private readonly IFileStorage _fileStorage;

        public CarService(ICarRepository carRepository, IFileStorage fileStorage)
        {
            _carRepository = carRepository;
            _fileStorage = fileStorage;
        }

        public async Task<Result<IEnumerable<CarDto>>> GetAllCarsAsync()
        {
            var cars = await _carRepository.GetAllAsync();
            var result = cars.Select(CarMapper.ToDto);
            return Result<IEnumerable<CarDto>>.Ok(result);
        }

        public async Task<Result<IEnumerable<CarDto>>> GetCarsByBrandIdAsync(int brandId)
        {
            var cars = await _carRepository.GetByBrandIdAsync(brandId);
            var result = cars.Select(CarMapper.ToDto);
            return Result<IEnumerable<CarDto>>.Ok(result);
        }

        public async Task<Result<CarDto>> GetCarByIdAsync(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
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
            await _carRepository.AddAsync(car);
            await _carRepository.SaveChangesAsync();

            return Result<int>.Ok(car.Id, "Car created successfully.");
        }

        public async Task<Result<string>> UpdateCarAsync(int id, CarDto dto, FileUploadDto? file)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
                return Result<string>.Fail("Car not found.");

            if (file != null)
            {
                if (!string.IsNullOrEmpty(car.ImageUrl))
                    await _fileStorage.DeleteFileAsync(car.ImageUrl);

                dto.ImageUrl = await _fileStorage.UploadFileAsync(file.Content!, file.FileName!, "uploads/car");
            }

            CarMapper.UpdateEntity(car, dto);
            await _carRepository.SaveChangesAsync();

            return Result<string>.Ok(null, "Car updated successfully.");
        }

        public async Task<Result<string>> DeleteCarAsync(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
                return Result<string>.Fail("Car not found.");

            if (!string.IsNullOrEmpty(car.ImageUrl))
                await _fileStorage.DeleteFileAsync(car.ImageUrl);

            _carRepository.DeleteAsync(car);
            await _carRepository.SaveChangesAsync();

            return Result<string>.Ok(null, "Car deleted successfully.");
        }
    }
}