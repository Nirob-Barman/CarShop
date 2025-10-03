using CarShop.Application.DTOs.Car;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Repositories;
using CarShop.Application.Mappers;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Services
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;

        public CarService(ICarRepository carRepository)
        {
            _carRepository = carRepository;
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

        public async Task<Result<int>> CreateCarAsync(CarDto dto)
        {
            var car = CarMapper.ToEntity(dto);
            await _carRepository.AddAsync(car);
            await _carRepository.SaveChangesAsync();

            return Result<int>.Ok(car.Id, "Car created successfully.");
        }

        public async Task<Result<string>> UpdateCarAsync(int id, CarDto dto)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
                return Result<string>.Fail("Car not found.");

            CarMapper.UpdateEntity(car, dto);
            await _carRepository.SaveChangesAsync();

            return Result<string>.Ok(null, "Car updated successfully.");
        }

        public async Task<Result<string>> DeleteCarAsync(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
                return Result<string>.Fail("Car not found.");

            _carRepository.DeleteAsync(car);
            await _carRepository.SaveChangesAsync();

            return Result<string>.Ok(null, "Car deleted successfully.");
        }
    }
}