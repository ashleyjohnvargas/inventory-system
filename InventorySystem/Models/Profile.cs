using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}
