using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Data;
using Nhom16_MVC.Repositories;
using Nhom16_MVC.Repositories.Interfaces;
using Nhom16_MVC.Services;
using Nhom16_MVC.Services.Interfaces;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });

// Services
builder.Services.AddSingleton<DatabaseService>();

builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<AvailableFieldService>();
builder.Services.AddScoped<SanBongService>();
builder.Services.AddScoped<SanBongChiTietService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<GiaoDichService>();
builder.Services.AddScoped<DanhGiaService>();
builder.Services.AddScoped<IDatSanService, DatSanService>();

builder.Services.AddHttpContextAccessor();

// Repository
builder.Services.AddScoped<ISanBongRepository, SanBongRepository>();
builder.Services.AddScoped<IBangGiaRepository, BangGiaRepository>();

// Business Service
builder.Services.AddScoped<ISanBongService, SanBongService>();
builder.Services.AddScoped<IBangGiaService, BangGiaService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


app.UseCors("AllowReact");

app.UseAuthorization();

app.MapControllers();

app.Run();