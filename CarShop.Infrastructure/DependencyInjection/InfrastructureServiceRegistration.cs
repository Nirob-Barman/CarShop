using CarShop.Application.DTOs.Email;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Interfaces.FileStorage;
using CarShop.Application.Interfaces.Identity;
using CarShop.Infrastructure.FileStorage;
using CarShop.Infrastructure.Payments;
using CarShop.Infrastructure.Identity;
using CarShop.Infrastructure.Persistence;
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
                //    Password = "",
                //    Ssl = true,
                //    AbortOnConnectFail = false,
                //    ConnectTimeout = 10000  // 10 seconds
                //};
                options.InstanceName = "CarShop:";
            });

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IRoleManager, RoleManager>();

            services.AddHttpClient();
            services.AddScoped<IUserContextService, UserContextService>();            

            services.Configure<EmailSettings>(config.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<IFileStorage, LocalFileStorage>();

            // Config encryption
            services.AddDataProtection();
            services.AddScoped<IConfigEncryptor, DataProtectionConfigEncryptor>();

            // Payment processors (strategy pattern)
            services.AddScoped<IPaymentProcessor, StripePaymentProcessor>();
            services.AddScoped<IPaymentProcessor, SSLCommerzPaymentProcessor>();
            services.AddScoped<IPaymentProcessor, BKashPaymentProcessor>();
            services.AddScoped<IPaymentProcessor, SurjoPayPaymentProcessor>();
            services.AddScoped<IPaymentProcessorFactory, PaymentProcessorFactory>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId     = config["Authentication:Google:ClientId"]!;
                    options.ClientSecret = config["Authentication:Google:ClientSecret"]!;
                    options.CallbackPath = "/google/callback";
                });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(14); // Persistent (Remember Me) cookie lasts 14 days
                options.SlidingExpiration = true; // Refresh cookie expiration on each request
                options.Cookie.HttpOnly = true; // Cookie can't be accessed by JavaScript
            });

            return services;
        }
    }
}
