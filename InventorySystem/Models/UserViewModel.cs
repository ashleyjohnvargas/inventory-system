using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using InventorySystem.Controllers;
using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class UserViewModel
    {
        public int Id { get; set; } // Primary Key
        public string? FullName { get; set; } // User's full name
        public string? Email { get; set; } // User's email
        public bool IsActive { get; set; } = true;
        // public string? Status { get; set; } // Use this field for Active/Inactive
        public string Status { get; set; } // For dropdown value

        public string? Password { get; set; } // Store hashed passwords
        //public string Status => IsActive ? "Active" : "Inactive";


    }
}
