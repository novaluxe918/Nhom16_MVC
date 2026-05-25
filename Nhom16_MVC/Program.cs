using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Services;
using System;
using Nhom16_MVC.Data;
using Nhom16_MVC.Services;


var builder = WebApplication.CreateBuilder(args);

// =========================
// Add services
// =========================

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<DatabaseService>();


builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<AvailableFieldService>();

//swagger
builder.Services.AddSwaggerGen();

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();


if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// API controllers
app.MapControllers();

// MVC controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();