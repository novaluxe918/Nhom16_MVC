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

// =========================
// Database & Enum Mapping
// =========================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.MapEnum<VaiTroEnum>("vai_tro_enum", nameTranslator: new NpgsqlNullNameTranslator());
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dataSource));

// =========================
// Add services
// =========================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// --- Helpers ---
builder.Services.AddScoped<EmailHelper>();
builder.Services.AddScoped<JwtHelper>();

// --- Core / Shared Services (từ phiên bản 1) ---
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<AvailableFieldService>();
builder.Services.AddScoped<SanBongService>();
builder.Services.AddScoped<SanBongChiTietService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<GiaoDichService>();
builder.Services.AddScoped<DanhGiaService>();

// --- Auth & Management Services (từ phiên bản 2) ---
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UserManagementService>();
builder.Services.AddScoped<StadiumManagementService>();
builder.Services.AddScoped<RatingManagementService>();
builder.Services.AddScoped<FinancialManagementService>();

// =========================
// JWT Authentication
// =========================
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

// =========================
// CORS (1 lần duy nhất, hỗ trợ cả 2 port)
// =========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// =========================================================================
// Middleware Pipeline (thứ tự chuẩn)
// =========================================================================

// 1. CORS phải đặt trước tất cả (xử lý Preflight Request)
app.UseCors("AllowReactApp");

// 2. Swagger (chỉ trong môi trường Development, 1 lần duy nhất)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 3. HTTPS Redirection
app.UseHttpsRedirection();

// 4. Static Files (phục vụ ảnh, CSS, JS tĩnh)
app.UseStaticFiles();

// 5. Routing
app.UseRouting();

// 6. Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// 7. Map Controllers
app.MapControllers();

app.Run();