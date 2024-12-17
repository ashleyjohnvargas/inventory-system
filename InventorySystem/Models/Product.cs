using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [Required]
        public required string Category { get; set; }
        public bool IsDeleted { get; set; } // New column for soft delete
        [Required]
        public DateTime DateAdded { get; set; } // New column for the date the product was added

    }
}
