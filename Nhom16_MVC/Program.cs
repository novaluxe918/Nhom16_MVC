using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Data;
using Nhom16_MVC.Repositories;
using Nhom16_MVC.Services;

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

app.Run();