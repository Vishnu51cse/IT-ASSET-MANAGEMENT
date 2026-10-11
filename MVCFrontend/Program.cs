using Backend.Data;
using Backend.Helpers;
using Backend.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services to DI container
builder.Services.AddControllersWithViews();

// Database & Services configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var sqliteConnectionString = builder.Configuration.GetConnectionString("SqliteConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    try
    {
        options.UseSqlServer(connectionString);
    }
    catch
    {
        options.UseSqlite(sqliteConnectionString);
    }
});

// Options & Services
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<IAssetRequestService, AssetRequestService>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IAssetHistoryService, AssetHistoryService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Cookie Authentication for MVC
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
