using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Data;
using Nhom16_MVC.Repositories;

using Nhom16_MVC.Services;
using Nhom16_MVC.Repositories.Interfaces;
using Nhom16_MVC.Services;
using Nhom16_MVC.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// =========================
// Add services
// =========================

builder.Services.AddControllers();

builder.Services.AddSingleton<DatabaseService>();

builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<AvailableFieldService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<ISanBongRepository, SanBongRepository>();

builder.Services.AddScoped<ISanBongService, SanBongService>();
builder.Services.AddScoped<IBangGiaRepository, BangGiaRepository>();
builder.Services.AddScoped<IBangGiaService, BangGiaService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
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

app.UseAuthorization();

// API Controllers
app.MapControllers();
app.UseCors("AllowReact");
app.Run();