using System.Reflection;
using CarShop.Application.Behaviors;
using CarShop.Application.Interfaces;
using CarShop.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CarShop.Application.DependencyInjection
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var applicationAssembly = Assembly.GetExecutingAssembly();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));
            services.AddValidatorsFromAssembly(applicationAssembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddScoped<IAuditLogService, AuditLogService>();

            return services;
        }
    }
}
