using CarShop.Application.Interfaces;
using CarShop.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CarShop.Application.DependencyInjection
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICarService, CarService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IOrderService, OrderService>();

            return services;
        }
    }
}
