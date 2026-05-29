using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Nhom16_MVC.Data;
using Nhom16_MVC.Helpers;
using Nhom16_MVC.Models.Enums;
using Nhom16_MVC.Services;
using Npgsql;
using Npgsql.NameTranslation;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174") // Các port Frontend của bạn
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});



// =========================
// Add services
// =========================

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

// Program.cs - sửa dòng MapEnum
dataSourceBuilder.MapEnum<VaiTroEnum>("vai_tro_enum", nameTranslator: new NpgsqlNullNameTranslator());
var dataSource = dataSourceBuilder.Build();

// Đăng ký DbContext sử dụng dataSource đã map Enum
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dataSource));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<EmailHelper>();
builder.Services.AddScoped<JwtHelper>();

// Đăng ký các tầng nghiệp vụ (Services) độc lập
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UserManagementService>();
builder.Services.AddScoped<StadiumManagementService>();
builder.Services.AddScoped<RatingManagementService>();
builder.Services.AddScoped<FinancialManagementService>();

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
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Đăng ký chính sách CORS dịch vụ
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// =========================================================================
// ⚡ THỨ TỰ MIDDLEWARE CHUẨN ĐÃ ĐƯỢC TỐI ƯU ⚡
// =========================================================================

// 1. Phải đặt CORS lên đầu tiên để duyệt qua Preflight Request
app.UseCors("AllowReactApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. Chuyển hướng HTTPS đặt phía dưới CORS ở môi trường dev
app.UseHttpsRedirection();

// 3. Định tuyến ứng dụng
app.UseRouting();

app.UseCors("AllowReactApp");
app.UseAuthorization();

// 5. Ánh xạ các Endpoint Controller
app.MapControllers();

app.Run();