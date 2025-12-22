using CarShop.Application.DTOs.Email;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Interfaces.FileStorage;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Interfaces.Repositories;
using CarShop.Application.Interfaces.Repositories.Integration;
using CarShop.Infrastructure.FileStorage;
using CarShop.Infrastructure.Identity;
using CarShop.Infrastructure.Persistence;
using CarShop.Infrastructure.Persistence.Repositories;
using CarShop.Infrastructure.Services;
using CarShop.Infrastructure.Services.Caching;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarShop.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config["Redis:ConnectionString"];
                //options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
                //{
                //    EndPoints = { "clear-moray-16822.upstash.io:6379" },
                //    Password = "AUG2AAIncDE3MjgyYmFmNWViMWI0YmY3YjBmNDA5ZjljNmEzMDI5YXAxMTY4MjI",
                //    Ssl = true,
                //    AbortOnConnectFail = false,
                //    ConnectTimeout = 10000  // 10 seconds
                //};
                options.InstanceName = "CarShop:";
            });

            services.AddScoped<IUserManager, IdentityUserManager>();
            services.AddScoped<ISignInManager, IdentitySignInManager>();
            services.AddScoped<IRoleManager, RoleManager>();

            services.AddHttpClient();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<ICarRepository, CarRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IIntegrationRepository, IntegrationRepository>();

            services.Configure<EmailSettings>(config.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<IFileStorage, LocalFileStorage>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie will expire after 30 minutes
                options.SlidingExpiration = true; // Refresh cookie expiration on each request
                options.Cookie.HttpOnly = true; // Cookie can't be accessed by JavaScript
            });

            return services;
        }
    }
}
