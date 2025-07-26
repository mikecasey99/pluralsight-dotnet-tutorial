// Loads configuration from appsettings.json, environment variables, etc.
// Also sets up Kestrel (default web server) and optionally IIS integration (on Windows)
// wwwroot folder will be the default folder to hold static content.
// Also gives us access to the services collection which helps us register with dependency injection
using BethanysPieShop.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// One instance per web request
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPieRepository, PieRepository>();

// Bringing in framework services that enable MVC.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BethanysPieShopDbContext>(options => {
    options.UseSqlite(
        builder.Configuration["ConnectionStrings:BethanysPieShopDbContextConnection"]);
});

var app = builder.Build();
// After builder.Build() is executed its when we can bring in our middleware components.

// Middleware components will be expressed typically with app.method()
// Example of middleware that will check wwwroot default folder.

app.UseStaticFiles();

// Show us private information that may help debugging.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Set defaults in MVC to route to our views.
app.MapDefaultControllerRoute();



DbInitializer.Seed(app);
// Starts our application.
app.Run();


