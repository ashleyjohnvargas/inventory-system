using Microsoft.AspNetCore.Mvc;
using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace InventorySystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ApplicationDbContext context, ILogger<LoginController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult AccessDenied()
        {
            return View();
        }


        // GET: Login
        public IActionResult LoginPage()
        {
            return View();
        }

        // POST: LoginPage (For handling login form submission)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginSubmit(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                _logger.LogError($"User not found: {email}");
                ViewBag.ErrorMessage = "Invalid email or password";
                return View("LoginPage");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning($"Inactive user login attempt: {email}");
                ViewBag.ErrorMessage = "Your account is inactive. Please contact support.";
                return View("LoginPage");
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                ViewBag.ErrorMessage = "Invalid email or password";
                return View("LoginPage");
            }

            // Update LastLogin timestamp
            user.LastLogin = DateTime.UtcNow;
            _context.SaveChanges();

            // Authentication using Cookies (Recommended over Session)
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim("LastPasswordChange", user.LastPasswordChange?.ToString("o") ?? "")

        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return RedirectToAction("Index", "Dashboard");
        }

        // GET: Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Prevent back navigation from showing cached content
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return RedirectToAction("LoginPage");
        }


        //public IActionResult Login(string ReturnUrl = null)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Dashboard"); // Already logged in, redirect to Dashboard
        //    }

        //    ViewBag.ReturnUrl = ReturnUrl;
        //    return View("LoginPage");
        //}



        //// POST: Login
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult LoginPage(string email, string password)
        //{
        //    var user = _context.Users.FirstOrDefault(u => u.Email == email);
        //    if (user == null)
        //    {
        //        _logger.LogError($"User not found: {email}");
        //        ViewBag.ErrorMessage = "Invalid credentials";
        //        return View("LoginPage");
        //    }
        //    if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))  // Compare the entered password with the hashed password
        //    {
        //        HttpContext.Session.SetString("UserId", user.Id.ToString());
        //        HttpContext.Session.SetString("UserFullName", user.FullName);
        //        return RedirectToAction("Index", "Dashboard");
        //    }

        //    ViewBag.ErrorMessage = "Invalid email or password";
        //    return View("LoginPage");
        //}


        // Background Service for checking inactive users and setting their last login date

        public class InactiveUserChecker : BackgroundService
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public InactiveUserChecker(IServiceScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                while (!stoppingToken.IsCancellationRequested)
                {
                    var threshold = DateTime.UtcNow.AddMonths(-6);
                    var inactiveUsers = dbContext.Users.Where(u => u.LastLogin < threshold && u.IsActive).ToList();

                    foreach (var user in inactiveUsers)
                    {
                        user.IsActive = false;
                    }

                    await dbContext.SaveChangesAsync();
                    await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Run daily
                }
            }
        }

        // Account Reactivation via Email
        //public async Task<IActionResult> ReactivateAccount(string email)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        //    if (user == null)
        //        return NotFound("User not found.");

        //    if (user.IsActive)
        //        return BadRequest("Account is already active.");

        //    user.IsActive = true;
        //    await _context.SaveChangesAsync();

        //    return Ok("Account reactivated.");
        //}


        // GET: Logout
        //public async Task<IActionResult> Logout()
        //{

        //    // Sign out the user and clear cookies
        //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    HttpContext.Session.Clear();
        //    // Redirect to login page

        //    return RedirectToAction("LoginPage", "Login");
        //}
    }
}
