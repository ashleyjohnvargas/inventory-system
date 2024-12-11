using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Models
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public List<Product> TopProductsByQuantity { get; set; }
        public List<Product> TopProductsByPrice { get; set; }
        public string TopCategory { get; set; }
    }
}
