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
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Register IShoppingCart with session-based factory method
builder.Services.AddScoped<IShoppingCart, ShoppingCart>(sp => ShoppingCart.GetCart(sp));

// Allows services to access the current HTTP context (needed for session, cart, etc.)
builder.Services.AddHttpContextAccessor();

// Enables session storage (used for shopping cart ID tracking)
builder.Services.AddSession();

// Bringing in framework services that enable MVC and Razor Pages.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Registering and configuring the SQLite database context
builder.Services.AddDbContext<BethanysPieShopDbContext>(options => {
    options.UseSqlite(
        builder.Configuration["ConnectionStrings:BethanysPieShopDbContextConnection"]);
});

var app = builder.Build();
// After builder.Build() is executed its when we can bring in our middleware components.

// Middleware components will be expressed typically with app.method()
// Example of middleware that will check wwwroot default folder.
app.UseStaticFiles();

// Enables session middleware so we can store/retrieve data per user
app.UseSession();

// Show us private information that may help debugging.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Set defaults in MVC to route to our views.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Also map Razor Pages (if used anywhere like for Identity or partial pages)
app.MapRazorPages();

// Seed the database with sample/test data if needed
DbInitializer.Seed(app);

// Starts our application.
app.Run();
