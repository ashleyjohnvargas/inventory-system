using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystem.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; } // Auto-incremented primary key

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } // Product name with a maximum length of 255

        public string? Description { get; set; } // Optional, allows variable-length text with no defined limit

        [Required]
        //[Range(0, (double)decimal.MaxValue)]
        public decimal Price { get; set; } // Decimal with precision 18, scale 2, must be non-negative

        [MaxLength(100)]
        public string? Color { get; set; } // Optional color with a maximum length of 100

        [MaxLength(50)]
        public string? Category { get; set; } // Optional category with a maximum length of 50

        //[Range(0, int.MaxValue)]
        public int OriginalStock { get; set; } // Non-negative integer for original stock

        //[Range(0, int.MaxValue)]
        public int CurrentStock { get; set; } // Non-negative integer for current stock

        [MaxLength(50)]
        public string? StockStatus { get; set; } // Optional stock status with a maximum length of 50

        public bool IsBeingSold { get; set; } = true; // Boolean field with a default of true (being sold)

        public bool IsDeleted { get; set; } = false; // Boolean field with a default of false (not deleted)

        [Required]
        public DateTime DateAdded { get; set; } = DateTime.Now; // Defaults to current date and time

        // Navigation property for related ProductImages
        public virtual ICollection<ProductImage> Images { get; set; }
    }
}
