using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public required string Email { get; set; }

        public bool IsActive { get; set; } = true;
        //public string? Status { get; set; } // Use this field for Active/Inactive
        [Required]
        [StringLength(255)]
        public required string Password { get; set; } // Store hashed passwords
        //public Profile Profile { get; set; } // Navigation Property

    }
}
