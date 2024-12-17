using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class CategoryStock
    {
        public string CategoryName { get; set; }
        public int StockQuantity { get; set; }
    }
}
