using Nhom16_MVC.Services;
<<<<<<< HEAD

var builder = WebApplication.CreateBuilder(args);

// =========================
// Add services
// =========================

=======
using Nhom16_MVC.Helpers;
using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Data;

var builder = WebApplication.CreateBuilder(args);

>>>>>>> feature/auth-login
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailHelper>();
builder.Services.AddScoped<JwtHelper>();

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Services
builder.Services.AddScoped<DatabaseService>();

builder.Services.AddScoped<StadiumService>();

var app = builder.Build();

// =========================
// Configure middleware
// =========================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
<<<<<<< HEAD

=======
>>>>>>> feature/auth-login
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.UseAuthorization();

// API controllers
app.MapControllers();

// MVC controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();