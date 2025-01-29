using Microsoft.AspNetCore.Mvc;
using InventorySystem.Models;
using Microsoft.EntityFrameworkCore;

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
      

        // GET: Login
        public IActionResult LoginPage()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LoginPage(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                _logger.LogError($"User not found: {email}");
                ViewBag.ErrorMessage = "Invalid email or password";
                return View("LoginPage");
            }
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))  // Compare the entered password with the hashed password
            {
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("UserFullName", user.FullName);
                return RedirectToAction("Index", "Dashboard");
            }
            
            ViewBag.ErrorMessage = "Invalid email or password";
            return View("LoginPage");
        }

        // GET: Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LoginPage", "Login");
        }
    }
}
