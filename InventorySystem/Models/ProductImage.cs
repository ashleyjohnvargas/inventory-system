using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystem.Models
{
    public class ProductImage
    {
        [Key]
        public int ImageId { get; set; } // Primary key

        [ForeignKey(nameof(Product))] // Specifies that ProductId links to the Product navigation property
        public int ProductId { get; set; } // Foreign key linking to Product

        public string FilePath { get; set; } // Path of the uploaded image

        // Navigation property for the related Product
        public virtual Product Product { get; set; }
    }
}
