
using CarShop.Application.DTOs.Car;
using CarShop.Application.DTOs.File;
using CarShop.Application.Wrappers;

namespace CarShop.Application.Interfaces
{
    public interface ICarService
    {
        Task<Result<IEnumerable<CarDto>>> GetAllCarsAsync();
        Task<Result<IEnumerable<CarDto>>> GetCarsByBrandIdAsync(int brandId);
        Task<Result<CarDto>> GetCarByIdAsync(int id);
        Task<Result<int>> CreateCarAsync(CarDto dto, FileUploadDto? file);
        Task<Result<string>> UpdateCarAsync(int id, CarDto dto, FileUploadDto? file);
        Task<Result<string>> DeleteCarAsync(int id);
        Task<Result<PagedResult<CarDto>>> SearchCarsAsync(CarSearchDto searchDto);
        Task<Result<IEnumerable<CarDto>>> GetCarsByIdsAsync(IEnumerable<int> ids);
        Task<Result<IEnumerable<CarDto>>> GetTopRatedCarsAsync(int count = 4);
        Task<Result<IEnumerable<CarDto>>> GetRecentCarsAsync(int count = 4);
    }
}
