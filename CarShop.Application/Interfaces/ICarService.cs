
using CarShop.Application.DTOs.Car;

namespace CarShop.Application.Interfaces
{
    public interface ICarService
    {
        Task<IEnumerable<CarDto>> GetAllCarsAsync();
        Task<IEnumerable<CarDto>> GetCarsByBrandIdAsync(int brandId);
        Task<CarDto> GetCarByIdAsync(int id);
        Task<int> CreateCarAsync(CarDto dto);
        Task UpdateCarAsync(int id, CarDto dto);
        Task DeleteCarAsync(int id);
    }
}
