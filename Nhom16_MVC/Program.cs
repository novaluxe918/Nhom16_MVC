using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Nhom16_MVC.Data;
using Nhom16_MVC.Helpers;
using Nhom16_MVC.Models.Enums;
using Nhom16_MVC.Services; 
using Npgsql;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

dataSourceBuilder.MapEnum<VaiTroEnum>("vai_tro_enum");
var dataSource = dataSourceBuilder.Build();

// Đăng ký DbContext sử dụng dataSource đã map Enum
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(dataSource));

builder.Services.AddControllers();
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

builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<EmailHelper>();
builder.Services.AddScoped<JwtHelper>();       

// Đăng ký các tầng nghiệp vụ (Services) độc lập
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UserManagementService>(); 

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
            ClockSkew = TimeSpan.Zero // Triệt tiêu thời gian chênh lệch để hết hạn token chính xác hơn
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<EmailHelper>();
builder.Services.AddScoped<JwtHelper>();       

// Đăng ký các tầng nghiệp vụ (Services) độc lập
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UserManagementService>();
builder.Services.AddScoped<StadiumManagementService>();

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
            ClockSkew = TimeSpan.Zero // Triệt tiêu thời gian chênh lệch để hết hạn token chính xác hơn
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<EmailHelper>();
builder.Services.AddScoped<JwtHelper>();       

// Đăng ký các tầng nghiệp vụ (Services) độc lập
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UserManagementService>();
builder.Services.AddScoped<StadiumManagementService>();
builder.Services.AddScoped<RatingManagementService>();

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
            ClockSkew = TimeSpan.Zero // Triệt tiêu thời gian chênh lệch để hết hạn token chính xác hơn
        };
    });

builder.Services.AddAuthorization();

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

app.UseCors("AllowReact");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();