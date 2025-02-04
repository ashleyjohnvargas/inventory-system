using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using InventorySystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace InventorySystem.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public SessionValidationMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var lastPasswordChangeClaim = context.User.FindFirst("LastPasswordChange")?.Value;

                if (userIdClaim != null && lastPasswordChangeClaim != null)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var userId = int.Parse(userIdClaim);
                        var dbUser = await dbContext.Users.FindAsync(userId);

                        // 🔴 If LastPasswordChange in DB is newer, force logout
                        if (dbUser != null && dbUser.LastPasswordChange > DateTime.Parse(lastPasswordChangeClaim))
                        {
                            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                            context.Response.Redirect("/Login/LoginPage");
                            return;
                        }
                    }
                }
            }

            await _next(context);
        }
    }


}
