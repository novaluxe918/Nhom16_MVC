using Nhom16_MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// =========================
// Add services
// =========================

builder.Services.AddControllersWithViews();

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