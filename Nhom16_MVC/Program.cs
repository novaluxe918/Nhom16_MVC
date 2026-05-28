using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Services;
using Nhom16_MVC.Data;

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

builder.Services.AddSingleton<DatabaseService>();

builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<AvailableFieldService>();

builder.Services.AddScoped<SanBongService>();
builder.Services.AddScoped<SanBongChiTietService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<GiaoDichService>();
builder.Services.AddScoped<DanhGiaService>();

builder.Services.AddHttpContextAccessor();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// =========================
// Middleware
// =========================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowReactApp");
app.UseAuthorization();

// API Controllers
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();