
using CarShop.Application.DTOs.Car;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Interfaces
{
    public interface ICarService
    {
        Task<Result<IEnumerable<CarDto>>> GetAllCarsAsync();
        Task<Result<IEnumerable<CarDto>>> GetCarsByBrandIdAsync(int brandId);
        Task<Result<CarDto>> GetCarByIdAsync(int id);
        Task<Result<int>> CreateCarAsync(CarDto dto);
        Task<Result<string>> UpdateCarAsync(int id, CarDto dto);
        Task<Result<string>> DeleteCarAsync(int id);
        //Task<IEnumerable<CarDto>> GetAllCarsAsync();
        //Task<IEnumerable<CarDto>> GetCarsByBrandIdAsync(int brandId);
        //Task<CarDto> GetCarByIdAsync(int id);
        //Task<int> CreateCarAsync(CarDto dto);
        //Task UpdateCarAsync(int id, CarDto dto);
        //Task DeleteCarAsync(int id);
    }
}
