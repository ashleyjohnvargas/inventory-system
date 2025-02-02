using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Services;
using static InventorySystem.Controllers.LoginController;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

//// Ecommerce service -- Deployed
//builder.Services.AddHttpClient<EcommerceService>(client =>
//{
//    client.BaseAddress = new Uri("https://gizmodeecommerce.azurewebsites.net/"); // Replace with Ecommerce System URL //
//});
//builder.Services.AddLocalization();


// Ecommerce service for Local host
builder.Services.AddHttpClient<EcommerceService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:44385/"); // Replace with Ecommerce System URL //
});
builder.Services.AddLocalization();


// Add services to the container.
// AddControllersWithViews() is added as a service because controllers in this system return Views
builder.Services.AddControllersWithViews();

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
// AddControllers() is different from AddControllersWithViews().
// AddControllers() is added as a service in this container because the controllers in the Controllers folder also
// contain methods or actions that are for API purposes, not just for returning Views.
// In this system, Inventory System, the ProductsController in the Controllers folder contain actions or methods both for returning
// Views and API routes. And since the ProductsController handles both MVC View routes and API routes, the AddControllers() service
// should be added in the container as you can see below. But if "ALL" the Controllers in a specific system are only used for
// returning Views, then AddControllers() doesn't need to be added in the containera anymore. It is added down below because
// the Contollers in this system, particularly in ProductsController, contains both MVC Views routes and API routes.
// If you observe ProductsController, it is inheriting from Controller class and not ControllerBase, because that contoller is used
// both for MVC Views routes and API routes. Meaning, it contain methods/actions that return Views, perform CRUDS operations,
// and methods/action for API routes/API endpoints, which are used by other system such as EcommerceSystem to connect or integrate
// with this system, the INventory system.
builder.Services.AddControllers(); 
// Ensure CORS (Cross-Origin Resource Sharing) is enabled to allow requests from the Ecommerce System.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowEcommerceSystem", policy =>
    {
        policy.WithOrigins("https://localhost:44385", "https://gizmodeecommerce.azurewebsites.net/") // Replace with your actual front-end URLs
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add background service for checking inactive users
builder.Services.AddHostedService<InactiveUserChecker>();

//builder.Services.AddControllersWithViews(options => {
//    options.Filters.Add(new AuthorizeFilter());
//});

// configure authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Login/LoginPage"; // Redirect to login if unauthorized
            options.LogoutPath = "/Logout";   // Redirect when logging out
            options.AccessDeniedPath = "/Login/AccessDenied"; // Handle access denied scenario
        });

// Add session services to the container
builder.Services.AddDistributedMemoryCache(); // Adds in-memory caching for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout as per your requirement
    options.Cookie.HttpOnly = true; // Only accessible by the server
    options.Cookie.IsEssential = true; // Necessary for session management
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        // Redirect to the login page if the user is not authenticated
        context.Response.Redirect("/Account/Login");
        return Task.CompletedTask;
    };
});

// Configure Kestrel to handle larger query strings by increasing max request body size
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024; // Set max body size to 10 KB (adjust as needed)
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Add this for development environment
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/Home/NotFound", "?code={0}"); // Handle 404 page
    app.UseHsts();
}

app.UseRequestLocalization("en-PH");

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Login/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowEcommerceSystem");

// Add session middleware to the request pipeline
app.UseSession(); // This should come before UseAuthorization
app.UseAuthentication(); // This should be placed before UseAuthorization
app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Login}/{action=LoginPage}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");


app.Run();
