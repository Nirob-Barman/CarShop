using CarShop.Domain.Entities;
using CarShop.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarShop.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<IntegrationSetting> IntegrationSettings { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<TestDriveBooking> TestDriveBookings { get; set; }
        public DbSet<StockAlert> StockAlerts { get; set; }
        public DbSet<AppNotification> AppNotifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<PaymentGateway> PaymentGateways { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply all IEntityTypeConfiguration from the current assembly
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // Brand-Car Relationship
            builder.Entity<Car>()
                .HasOne(c => c.Brand)
                .WithMany(b => b.Cars)
                .HasForeignKey(c => c.BrandId)
                .OnDelete(DeleteBehavior.Cascade);

            // Car-Comment Relationship
            builder.Entity<Comment>()
                .HasOne(c => c.Car)
                .WithMany(car => car.Comments)
                .HasForeignKey(c => c.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            // Car-Order Relationship
            builder.Entity<Order>()
                .HasOne(o => o.Car)
                .WithMany()
                .HasForeignKey(o => o.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            // Car-WishlistItem Relationship
            builder.Entity<WishlistItem>()
                .HasOne(w => w.Car)
                .WithMany()
                .HasForeignKey(w => w.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            // Car-TestDriveBooking Relationship
            builder.Entity<TestDriveBooking>()
                .HasOne(t => t.Car)
                .WithMany()
                .HasForeignKey(t => t.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            // Car-StockAlert Relationship
            builder.Entity<StockAlert>()
                .HasOne(s => s.Car)
                .WithMany()
                .HasForeignKey(s => s.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            // configure decimal precision
            builder.Entity<Car>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Order>()
                .Property(o => o.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Order>()
                .Property(o => o.FinalPrice)
                .HasColumnType("decimal(18,2)");

            builder.Entity<PromoCode>()
                .Property(p => p.DiscountPercent)
                .HasColumnType("decimal(18,2)");

            builder.Entity<PromoCode>()
                .Property(p => p.MaxDiscountAmount)
                .HasColumnType("decimal(18,2)");

            // PaymentGateway -> Order relationship
            builder.Entity<Order>()
                .HasOne(o => o.PaymentGateway)
                .WithMany(g => g.Orders)
                .HasForeignKey(o => o.PaymentGatewayId)
                .OnDelete(DeleteBehavior.SetNull);

            // PaymentTransaction -> Order relationship
            builder.Entity<PaymentTransaction>()
                .HasOne(t => t.Order)
                .WithMany()
                .HasForeignKey(t => t.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // PaymentTransaction -> PaymentGateway relationship
            builder.Entity<PaymentTransaction>()
                .HasOne(t => t.PaymentGateway)
                .WithMany(g => g.Transactions)
                .HasForeignKey(t => t.PaymentGatewayId)
                .OnDelete(DeleteBehavior.Restrict);

            // PaymentTransaction decimal precision
            builder.Entity<PaymentTransaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");
        }
    }
}
