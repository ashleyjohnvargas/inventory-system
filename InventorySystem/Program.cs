using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Inventory service
builder.Services.AddHttpClient<EcommerceService>(client =>
{
    client.BaseAddress = new Uri("https://gizmodeecommerce.azurewebsites.net/"); // Replace with Inventory System URL
});


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
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Add session services to the container
builder.Services.AddDistributedMemoryCache(); // Adds in-memory caching for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout as per your requirement
    options.Cookie.HttpOnly = true; // Only accessible by the server
    options.Cookie.IsEssential = true; // Necessary for session management
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
    app.UseHsts();
}

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=LoginPage}/{id?}");

app.Run();
