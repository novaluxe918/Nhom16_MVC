using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Nhom16_MVC.Data;
using Nhom16_MVC.Helpers;
using Nhom16_MVC.Models.Enums; // Đảm bảo namespace này chứa VaiTroEnum
using Npgsql;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình NpgsqlDataSource để Map Enum (Cách duy nhất để Postgres hiểu kiểu dữ liệu custom)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
// Map tất cả các Enum bạn có trong SQL vào đây
dataSourceBuilder.MapEnum<VaiTroEnum>("vai_tro_enum");
// Ví dụ nếu bạn có thêm các enum khác:
// dataSourceBuilder.MapEnum<TrangThaiDatEnum>("trang_thai_dat");
var dataSource = dataSourceBuilder.Build();

// 2. Đăng ký DbContext với dataSource đã được cấu hình Enum

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(dataSource));
builder.Services.AddControllers();
builder.Services.AddScoped<EmailHelper>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<Nhom16_MVC.Services.IAuthService, Nhom16_MVC.Services.AuthService>();

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "Chon_Mot_Chuoi_Key_That_Dai_Va_Bao_Mat_Nhom16_SportSync_2026";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();


builder.Services.AddScoped<ISanBongRepository, SanBongRepository>();

builder.Services.AddScoped<ISanBongService, SanBongService>();
builder.Services.AddScoped<IBangGiaRepository, BangGiaRepository>();
builder.Services.AddScoped<IBangGiaService, BangGiaService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();