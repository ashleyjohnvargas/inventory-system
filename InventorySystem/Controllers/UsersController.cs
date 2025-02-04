using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Models;
using BCrypt;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;


namespace InventorySystem.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View(); // Redirect to an access-denied page
        }

        [Authorize] //(Roles = "Admin")
        public async Task<IActionResult> Users()
        {
            try
            {
                // Fetch data from the users table
                var activeUsers = await _context.Users
                    //.Where(u => u.IsActive == true) // Filter only active users
                    .Select(u => new UserViewModel
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        Email = u.Email,
                        IsActive = u.IsActive,

                    })
                    .ToListAsync();

                // Pass the list of users to the view
                return View(activeUsers);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading users: {ex.Message}";
                return View(new List<UserViewModel>()); // Return an empty list in case of error
            }
        }

       // GET: Display the Add User form
       [HttpGet]
       [Authorize]
        public IActionResult AddUser()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(string FullName, string Email, string Password, bool IsActive)
        {
            if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                TempData["ErrorMessage"] = "All fields are required.";
                return RedirectToAction("AddUser");
            }
            try
            {
                // Check if the user already exists by email
                var existingUser = await _context.Users
                                                 .FirstOrDefaultAsync(u => u.Email == Email);
                if (existingUser != null)
                {
                    TempData["ErrorMessage"] = "A user with this email already exists.";
                    return RedirectToAction("AddUser");
                }

                // Hash the password before saving
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);

                var newUser = new User
                {
                    FullName = FullName,
                    Email = Email,
                   // Status = Status,  // "Active" or "Inactive"
                    Password = hashedPassword,  // Store the hashed password
                    IsActive = IsActive = true  // Set the IsActive status based on input


                };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                //try
                //{
                //    var newUserProfile = new Profile
                //    {
                //        Id = newUser.Id,  // Link the profile to the newly created user
                //        FullName = newUser.FullName,
                //        Email = newUser.Email,
                //    };

                //    _context.Profiles.Add(newUserProfile);
                //    await _context.SaveChangesAsync();

                //    TempData["SuccessMessage"] = "User added successfully!";
                //    return RedirectToAction("Users"); // Redirect to the users list or another page
                //}
                //catch (Exception ex)
                //{
                //    TempData["ErrorMessage"] = "An error occurred while adding the user: " + ex.Message;
                //    return RedirectToAction("AddUser");
                //}
             

                // Create the profile after saving the user
                //var newUserProfile = new Profile
                //{
                //    Id = newUser.Id,  // Link the profile to the newly created user
                //    FullName = newUser.FullName,
                //    Email = newUser.Email,
                //};

                //_context.Profiles.Add(newUserProfile);
                //await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User added successfully!";
                return RedirectToAction("Users"); // Redirect to the users list or another page
            }
            catch (Exception ex)
            {
                // Log the detailed exception for debugging purposes
                TempData["ErrorMessage"] = "An error occurred while adding the user: " + ex.Message;
                return RedirectToAction("AddUser");
            }
        }


        // Display the Edit User page
        [Authorize]
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Users");
            }

            // Pass the user data to the view
            var model = new UserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                IsActive = user.IsActive
            };

            return View(model);
        }


        // Handle form submission to update user
        // Handle form submission to update user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == model.Id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Users");
            }

            // Disable the foreign key constraint temporarily
            //_context.Database.ExecuteSqlRaw("ALTER TABLE Profiles NOCHECK CONSTRAINT FK_UserProfiles_Users_Email");

            try
            {
                // Update the email in UserProfiles first
                var userProfile = _context.Profiles.FirstOrDefault(up => up.Email == user.Email);
                if (userProfile != null)
                {
                    // Update the email in UserProfiles
                    userProfile.Email = model.Email;
                }

                // Update user details
                user.FullName = model.FullName;
                user.Email = model.Email; // Update email in Users table after UserProfiles
                user.IsActive = model.Status == "Active"; // Map dropdown value to boolean

                // Update password if provided
                if (!string.IsNullOrEmpty(model.Password))
                {
                    //user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                    user.LastPasswordChange = DateTime.UtcNow; // Update timestamp
                    _context.SaveChanges();

                    // 🔴 Force logout the user whose password was changed
                    //await ForceLogoutUser(user);
                    //return RedirectToAction("LoginPage", "Login"); // Redirect to login
                    return RedirectToAction("Users"); // Redirect to login


                }

                _context.SaveChanges();

                TempData["SuccessMessage"] = "User updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error occurred: {ex.Message}";
            }
            //finally
            //{
            //    // Re-enable the foreign key constraint and validate data integrity
            //    //_context.Database.ExecuteSqlRaw("ALTER TABLE Profiles WITH CHECK CHECK CONSTRAINT FK_UserProfiles_Users_Email");
            //}

            return RedirectToAction("Users");
        }
        // Method to force logout the user whose password was changed
        private async Task ForceLogoutUser(User user)
        {
            // Sign out the specific user whose password was updated by clearing their cookies
            var authenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            // Sign out the user explicitly by removing the cookie for this user
            await HttpContext.SignOutAsync(authenticationScheme);

            // You may also want to invalidate or clear the user's authentication cookie on the client side
            // (optional, but might help in some situations):
            Response.Cookies.Delete(".AspNetCore.Cookies"); // Adjust cookie name if needed

            // This ensures the user must log in again
        }
        // Custom method to force sign out the user whose password was updated
        private async Task SignOutSpecificUser(User user)
        {
            // You can sign out the user explicitly by removing their authentication cookie
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                // Add other claims as needed
            }, CookieAuthenticationDefaults.AuthenticationScheme));

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        }


        // Force logout on Password Change (Admin)
        // After the admin updates the password, force logout all active sessions:
        public async Task<IActionResult> ForceLogout(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.LastPasswordChange = DateTime.UtcNow; // 🔹 Mark session invalid
            await _context.SaveChangesAsync(); // Save to DB

            return RedirectToAction("Users"); // ✅ Redirect admin, no logout!
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Set IsActive to false (mark as inactive)
            user.IsActive = false;
            _context.Update(user);
            await _context.SaveChangesAsync();

            // Optionally, show a success message
            TempData["SuccessMessage"] = "User marked as inactive successfully.";

            return RedirectToAction("Users");
        }



    }

}