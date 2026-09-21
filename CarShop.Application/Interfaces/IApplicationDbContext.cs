using CarShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Car> Cars { get; }
    DbSet<Brand> Brands { get; }
    DbSet<Order> Orders { get; }
    DbSet<Comment> Comments { get; }
    DbSet<IntegrationSetting> IntegrationSettings { get; }
    DbSet<WishlistItem> WishlistItems { get; }
    DbSet<PromoCode> PromoCodes { get; }
    DbSet<TestDriveBooking> TestDriveBookings { get; }
    DbSet<StockAlert> StockAlerts { get; }
    DbSet<AppNotification> AppNotifications { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<PaymentGateway> PaymentGateways { get; }
    DbSet<PaymentTransaction> PaymentTransactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
