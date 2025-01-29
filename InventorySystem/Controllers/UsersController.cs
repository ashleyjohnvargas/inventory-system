using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Models;
using BCrypt;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace InventorySystem.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }
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
        public IActionResult AddUser()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddUser(string FullName, string Email, string Password, bool IsActive)
        {
            if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                TempData["ErrorMessage"] = "All fields are required.";
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
                IsActive = IsActive = true// Set the IsActive based on the status


            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            try
            {

               


                var newUserProfile = new Profile
                {
                    Id = newUser.Id,  // Link the profile to the newly created user
                    FullName = newUser.FullName,
                    Email = newUser.Email,
                };

                _context.Profiles.Add(newUserProfile);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User added successfully!";
                return RedirectToAction("Users"); // Redirect to the users list or another page
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while adding the user: " + ex.Message;
                return RedirectToAction("AddUser");
            }

        }

        // Display the Edit User page
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
        [HttpPost]
        public IActionResult EditUser(UserViewModel model)
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
                    user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
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