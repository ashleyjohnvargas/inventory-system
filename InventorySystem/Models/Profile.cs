using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
       
        //[ForeignKey("User")]
       //public int Id { get; set; }  // Foreign Key
        //public User User { get; set; }  // Navigation Property

    }
}
