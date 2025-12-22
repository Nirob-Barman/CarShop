using CarShop.Application.DTOs.Order;
using CarShop.Web.ViewModels.Order;

namespace CarShop.Web.ViewModels.Mappers
{
    public static class OrderMapper
    {
        public static OrderViewModel ToViewModel(OrderDto dto)
        {
            return new OrderViewModel
            {
                Id = dto.Id,
                CarTitle = dto.CarTitle,
                CarPrice = dto.CarPrice,
                CarImageUrl = dto.CarImageUrl,
                Quantity = dto.Quantity,
                TotalPrice = dto.TotalPrice,
                OrderedAt = dto.OrderedAt
            };
        }

        public static IEnumerable<OrderViewModel> ToViewModels(IEnumerable<OrderDto> dtos)
        {
            return dtos.Select(ToViewModel);
        }
    }
}
