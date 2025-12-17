using Microsoft.EntityFrameworkCore;
using Web_BanHang.Models; // Namespace này phải trùng với tên Project của bạn

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. ĐĂNG KÝ KẾT NỐI DATABASE
// ==========================================
builder.Services.AddDbContext<BanhangdbContext>(options =>
    options.UseSqlServer("Server=DESKTOP-O8PU7IU\\SQLEXPRESS;Database=BANHANGDB;Trusted_Connection=True;TrustServerCertificate=True;"));

// 2. Đăng ký dịch vụ Controller (để viết API)
builder.Services.AddControllers();

// 3. Đăng ký Swagger (Giao diện test API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ==========================================
// 4. CẤU HÌNH PIPELINE (LUỒNG CHẠY)
// ==========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers(); // Quan trọng: Để code tìm thấy các Controller

app.Run();