using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Car.Commands.DeleteCar
{
    public record DeleteCarCommand(int Id) : IRequest<Result<string>>;
}
