using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Entities.Models;
using Repository.Data;
using Repository.Interfaces;
using Repository.Implementations;
using magazine_app.Services;
using magazine_app.Services.Interfaces;
using magazine_app.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    // Use SameAsRequest to allow cookies on both HTTP and HTTPS
    // This fixes the IIS login issue where cookies weren't being set on HTTP
    // When using HTTPS, cookies will automatically be secure
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Add Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin", "SuperAdmin"));
    options.AddPolicy("UploaderAccess", policy => policy.RequireRole("Admin", "SuperAdmin", "Uploader"));
    options.AddPolicy("CustomerAccess", policy => policy.RequireRole("Customer", "Admin", "SuperAdmin"));
});

// Register Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

// Register Services
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IGuideService, GuideService>();

// Add MVC Controllers with Views
builder.Services.AddControllersWithViews();

// Add Razor Pages (for Identity UI if needed)
builder.Services.AddRazorPages();

var app = builder.Build();

// Seed database with roles and super admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DataSeed.SeedAsync(services, builder.Configuration);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Enable static files for uploads
app.UseStaticFiles();

app.UseRouting();

// Add Request Logging Middleware
app.UseRequestLogging();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();
