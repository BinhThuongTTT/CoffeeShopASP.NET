using CoffeeShopWeb.Models;
using CoffeeShopWeb.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization; // Add this namespace for CultureInfo

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<Context>()
    .AddDefaultTokenProviders();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"));
    options.AddPolicy("StaffOnly", policy => policy.RequireRole("Staff"));
});
builder.Services.ConfigureApplicationCookie(options => options.Cookie.SecurePolicy = CookieSecurePolicy.Always);

builder.Services.AddScoped<ICoffeeShopMenuService, CoffeeShopMenuService>();
builder.Services.AddScoped<ICoffeeShopStaffService, CoffeeShopStaffService>();
builder.Services.AddScoped<ICoffeeShopManagerService, CoffeeShopManagerService>();
builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IStaffBillingService, StaffBillingService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

// Configure localization for vi-VN culture
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("vi-VN") // Use CultureInfo object instead of string
    };
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("vi-VN");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRequestLocalization(); // Apply culture settings before routing
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=CoffeeShop}/{action=Index}/{id?}");
app.Run();