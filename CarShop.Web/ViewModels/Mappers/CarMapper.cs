using CarShop.Application.DTOs.Car;
using CarShop.Web.ViewModels.Car;

namespace CarShop.Web.ViewModels.Mappers
{
    public static class CarMapper
    {
        public static CarViewModel ToViewModel(CarDto dto)
        {
            if (dto == null) return null!;

            return new CarViewModel
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                BrandId = dto.BrandId,
                BrandName = dto.BrandName,
                ImageUrl = dto.ImageUrl
            };
        }

        public static IEnumerable<CarViewModel> ToViewModels(IEnumerable<CarDto> dtos)
        {
            return dtos.Select(ToViewModel);
        }
    }
}
