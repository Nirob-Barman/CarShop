using CarShop.Application.DTOs.Email;
using CarShop.Application.Interfaces;
using CarShop.Application.Interfaces.Cache;
using CarShop.Application.Interfaces.FileStorage;
using CarShop.Application.Interfaces.Identity;
using CarShop.Application.Interfaces.Repositories;
using CarShop.Application.Interfaces.Repositories.Integration;
using CarShop.Application.Services;
using CarShop.Infrastructure.FileStorage;
using CarShop.Infrastructure.Identity;
using CarShop.Infrastructure.Persistence;
using CarShop.Infrastructure.Persistence.Repositories;
using CarShop.Infrastructure.Services;
using CarShop.Infrastructure.Services.Caching;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
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

builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IFileStorage, LocalFileStorage>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<ImageService, ImageService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();


builder.Services.AddScoped<IUserManager, IdentityUserManager>();
builder.Services.AddScoped<ISignInManager, IdentitySignInManager>();
builder.Services.AddScoped<IRoleManager, RoleManager>();


builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddScoped<IIntegrationRepository, IntegrationRepository>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie will expire after 30 minutes
    options.SlidingExpiration = true; // Refresh cookie expiration on each request
    options.Cookie.HttpOnly = true; // Cookie can't be accessed by JavaScript
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
