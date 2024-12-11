using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; } // Consider using hashing for security
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}
