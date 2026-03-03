using System;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.DataProtection;
using Web_BanHang.Models; // Namespace này phải trùng với tên Project của bạn
using Web_BanHang.Data;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. ĐĂNG KÝ KẾT NỐI DATABASE
// ==========================================
// Register both the legacy `BanhangdbContext` (compat wrapper) and the scaffolded `FashionEcommerceDbContext` so DI resolves either type.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FashionEcommerceDbContext>(options =>
    options.UseSqlServer(connectionString));

// Also register BanhangdbContext so existing controllers/services depending on it continue to work.
builder.Services.AddDbContext<BanhangdbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Identity - ApplicationUser và Role
builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password policy (stronger)
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Lockout settings
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

    // User & sign-in
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true; // require email confirmation by default
})
.AddEntityFrameworkStores<ApplicationIdentityDbContext>()
.AddDefaultTokenProviders();

// Increase PBKDF2 iteration count for PasswordHasher
builder.Services.Configure<PasswordHasherOptions>(options =>
{
    // Default varies by framework; increase to make brute-force harder (balance CPU cost and UX)
    options.IterationCount = 150_000;
});

// Configure cookie security
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Data protection key persistence (so auth tokens survive app restarts)
var keysDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Web_BanHang", "DataProtection-Keys");
Directory.CreateDirectory(keysDir);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysDir))
    .SetApplicationName("Web_BanHang");

// Configure token lifespans (email confirmation / reset)
builder.Services.Configure<Microsoft.AspNetCore.Identity.DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(3));

// JWT configuration
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection.GetValue<string>("Key");
var jwtIssuer = jwtSection.GetValue<string>("Issuer");
var jwtAudience = jwtSection.GetValue<string>("Audience");
var jwtExpire = jwtSection.GetValue<int>("ExpireMinutes");

if (string.IsNullOrEmpty(jwtKey) || jwtKey == "REPLACE_WITH_STRONG_SECRET_KEY")
{
    // Warning for developer; in production replace with strong key stored in secrets
    Console.WriteLine("WARNING: Jwt:Key is empty or default. Replace it with a secure secret before production.");
}
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? ""));

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

// 3. Register MVC + RazorPages for simple frontend HTML/CSS
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 4. (Swagger temporarily disabled due to package conflicts)

// Email sender (simple Smtp/File fallback) - see Services/EmailSender.cs
builder.Services.AddTransient<Web_BanHang.Services.IEmailSender, Web_BanHang.Services.EmailSender>();

// 5. Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", p => p.RequireRole("Admin"));
    options.AddPolicy("RequireStaff", p => p.RequireRole("Staff"));
    options.AddPolicy("RequireCustomer", p => p.RequireRole("Customer"));
});

var app = builder.Build();

// ==========================================
// 6. CẤU HÌNH PIPELINE (LUỒNG CHẠY)
// ==========================================
// Note: Swagger/OpenAPI assembly caused a runtime type load issue in this environment.
// Temporarily disable Swagger at startup to allow the app to run; we will fix package
// versions for a permanent solution.
//
// If you want Swagger during development, re-enable the block below after updating
// the `Swashbuckle.AspNetCore` / `Microsoft.OpenApi` package versions.

/*
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
*/

// Show detailed exceptions in Development to help debugging local errors
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Seed roles + admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.InitializeAsync(services).GetAwaiter().GetResult();
}

app.Run();