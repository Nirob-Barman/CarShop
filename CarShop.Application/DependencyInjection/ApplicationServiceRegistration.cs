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
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IPromoCodeService, PromoCodeService>();
            services.AddScoped<ITestDriveService, TestDriveService>();
            services.AddScoped<IStockAlertService, StockAlertService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddScoped<IBulkImportService, BulkImportService>();
            services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();
            services.AddScoped<IPaymentService, PaymentService>();

            return services;
        }
    }
}
