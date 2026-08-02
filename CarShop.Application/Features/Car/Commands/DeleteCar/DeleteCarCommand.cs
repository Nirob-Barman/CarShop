using CarShop.Application.Wrappers;
using MediatR;

namespace CarShop.Application.Features.Car.Commands.DeleteCar
{
    public class DeleteCarCommand : IRequest<Result<string>>
    {
        public int Id { get; set; }

        public DeleteCarCommand(int id)
        {
            Id = id;
        }
    }
}
