using InventorySystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace InventorySystem.Controllers
{
    public class BaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var lastPasswordChangeClaim = User.FindFirst("LastPasswordChange")?.Value;

                if (!string.IsNullOrEmpty(userIdClaim) && !string.IsNullOrEmpty(lastPasswordChangeClaim))
                {
                    var dbUser = _context.Users.Find(int.Parse(userIdClaim));

                    if (dbUser != null && dbUser.LastPasswordChange > DateTime.Parse(lastPasswordChangeClaim))
                    {
                        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
                        context.Result = RedirectToAction("LoginPage", "Login");
                    }
                }
            }

            base.OnActionExecuting(context);
        }
    }
}