using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}