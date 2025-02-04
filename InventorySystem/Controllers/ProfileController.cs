using Microsoft.AspNetCore.Mvc;
using InventorySystem.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace InventorySystem.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProfileController> _logger;
        public ProfileController(ApplicationDbContext context, ILogger<ProfileController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Display the profile page
        [Authorize]
        public IActionResult ProfilePage()
        {
            // Retrieve the logged-in user's ID from the claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get the user ID from claims

            if (string.IsNullOrEmpty(userId))
            {
                // If the user ID is not found in claims, redirect to the login page
                TempData["ErrorMessage"] = "User profile not found. Please login again.";
                return RedirectToAction("LoginPage", "Login");
            }

            // Log the UserId to the console (for debugging purposes)
            _logger.LogInformation($"Logged-in UserId: {userId}");

            // Fetch user profile and user details from the database
            var profile = _context.UserProfiles.FirstOrDefault(p => p.Id == int.Parse(userId)); // Use the correct relationship key
            var user = _context.Users.FirstOrDefault(u => u.Id == int.Parse(userId));

            if (profile == null)
            {
                // If the profile does not exist, create a default profile for new users
                profile = new UserProfile
                {
                    //Id = int.Parse(userId),  // Ensure the profile is linked to the correct user
                    FullName = user?.FullName ?? "",  // Default to user's name if available
                    Email = user?.Email ?? "",
                    PhoneNumber = "",
                    Address = ""
                };
                _context.UserProfiles.Add(profile);
                _context.SaveChanges();
            }

            return View(profile);  // Pass the profile to the view
        }

        [Authorize]
        public IActionResult EditProfile()
        {
            // Retrieve the logged-in user's ID from claims (instead of session)
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                // If no valid UserId is found in the claims, redirect to login page
                _logger.LogWarning("EditProfile: No valid UserId found in claims. Redirecting to login.");
                return RedirectToAction("LoginPage", "Login");
            }

            // Fetch the profile for the logged-in user
            var profile = _context.UserProfiles.FirstOrDefault(p => p.Id == userId);
            if (profile == null)
            {
                _logger.LogWarning($"EditProfile: No profile found for UserId {userId}. Redirecting to login.");
                TempData["ErrorMessage"] = "Profile not found.";
                return RedirectToAction("LoginPage", "Login");
            }

            // Return the profile to the view for editing
            return View(profile);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult UpdateProfile(Profile model)
        {
            // Validate the model
            if (!ModelState.IsValid)
            {
                return View("EditProfile", model);
            }

            // Retrieve the logged-in user's ID from claims (instead of session)
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                // If no valid UserId is found in claims, redirect to login page
                return RedirectToAction("LoginPage", "Login");
            }

            // Fetch the profile and user
            var profile = _context.UserProfiles.FirstOrDefault(p => p.Id == userId);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (profile != null && user != null)
            {
                // Update the profile with the new data from the model
                profile.PhoneNumber = model.PhoneNumber;
                profile.Address = model.Address;
                profile.Email = model.Email;

                // Update the user's email (if necessary)
                user.Email = model.Email;

                // Optionally, update other fields like FullName if needed
                // user.FullName = model.FullName;

                // Save changes to the database
                _context.SaveChanges();

                // Update session with the new email (if necessary)
                HttpContext.Session.SetString("UserEmail", user.Email);

                // Set a success message and redirect
                TempData["SuccessMessage"] = "Profile updated successfully!";
            }

            return RedirectToAction("ProfilePage");
        }

        // Display the profile page
        //[Authorize]
        //public IActionResult ProfilePage()
        //{
        //    // Retrieve the logged-in user's ID from session (as string) and convert to int
        //    var userIdString = HttpContext.Session.GetString("UserId");
        //    if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        //    {
        //        // If the session doesn't contain a valid UserId, redirect to login page
        //        //TempData["ErrorMessage"] = "User profile not found.";
        //        return RedirectToAction("LoginPage", "Login");
        //    }

        //    // Log the UserId to the console (this will output to the console/logs)
        //    _logger.LogInformation($"Logged-in UserId: {userId}");


        //    // Fetch user profile and user details
        //    var profile = _context.UserProfiles.FirstOrDefault(p => p.Id == userId);
        //    var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        //    if (profile == null)
        //    {
        //        // Create a default profile for new users
        //        profile = new UserProfile
        //        {
        //            FullName = "",
        //            Email = "",
        //            PhoneNumber = "",
        //            Address = ""
        //        };
        //        _context.UserProfiles.Add(profile);
        //        _context.SaveChanges();
        //    }
        //    return View(profile);
        //}

        //[Authorize]
        //public IActionResult EditProfile()
        //{
        //    // Retrieve the logged-in user's ID from session (as string) and convert to int
        //    var userIdString = HttpContext.Session.GetString("UserId");
        //    if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        //    {
        //        // If the session doesn't contain a valid UserId, redirect to login page
        //        _logger.LogWarning("EditProfile: No valid UserId found in session. Redirecting to login.");
        //        return RedirectToAction("LoginPage", "Login");
        //    }
        //    var profile = _context.UserProfiles.FirstOrDefault(p => p.Id == userId);
        //    if (profile == null)
        //    {
        //        _logger.LogWarning($"EditProfile: No profile found for UserId {userId}. Redirecting to login.");
        //        TempData["ErrorMessage"] = "Profile not found.";
        //        return RedirectToAction("LoginPage", "Login");
        //    }
        //    return View(profile);
        //}

        //// Update profile
        //[ValidateAntiForgeryToken]
        //[HttpPost]
        //public IActionResult UpdateProfile(Profile model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View("EditProfile", model);
        //    }

        //    // Retrieve the logged-in user's ID from session (as string) and convert to int
        //    var userIdString = HttpContext.Session.GetString("UserId");
        //    if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        //    {
        //        // If the session doesn't contain a valid UserId, redirect to login page
        //        return RedirectToAction("LoginPage", "Login");
        //    }

        //    var profile = _context.UserProfiles.FirstOrDefault(p => p.Id == userId);
        //    var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        //    if (profile != null && user != null)
        //    {
        //        // Update profile information
        //        profile.FullName = user.FullName;
        //        profile.PhoneNumber = model.PhoneNumber;
        //        profile.Address = model.Address;
        //        profile.Email = model.Email;

        //        // Update email if changed
        //        //user.FullName = model.FullName;
        //        user.Email = model.Email;
        //        // Update session with the new email
        //        HttpContext.Session.SetString("UserEmail", user.Email);

        //        //HttpContext.Session.SetString("UserFullName", user.FullName);

        //        _context.SaveChanges();
        //        TempData["SuccessMessage"] = "Profile updated successfully!";
        //    }

        //    return RedirectToAction("ProfilePage");
        //}

        // Display the EditPasswordPage
        [Authorize]
        public IActionResult ChangePasswordPage()
        {
            return View();
        }

        // Change or Update the password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            // Retrieve the logged-in user's ID from session (as string) and convert to int
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("LoginPage", "Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("ChangePasswordPage");
            }

            //// Check if the current password matches the user's password (this is without hashing)
            //if (user.Password != currentPassword)
            //{
            //    TempData["ErrorMessage"] = "Current password is incorrect.";
            //    return RedirectToAction("ChangePasswordPage");
            //}
            // Check if the current password matches the hashed password (using BCrypt)
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))  // Compare hashed password
            {
                TempData["ErrorMessage"] = "Current password is incorrect.";
                return RedirectToAction("ChangePasswordPage");
            }

            // Check if the new password and confirm password match
            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "New password and confirm password do not match.";
                return RedirectToAction("ChangePasswordPage");
            }

            // Hash the new password before saving it to the database
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);  // Hash the new password


            _context.SaveChanges();

            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction("ProfilePage");
        }

    }
}