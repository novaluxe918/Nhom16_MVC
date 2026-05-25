using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Services;
using Nhom16_MVC.Data;

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