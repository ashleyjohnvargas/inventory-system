using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InventorySystem.Models;
using System.Linq;

namespace InventorySystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Total number of products
            var totalProducts = _context.Products.Count(p => !p.IsDeleted);

            // Top 3 products with the highest quantity
            var topProductsByQuantity = _context.Products
                .Where(p => !p.IsDeleted)  // Exclude deleted products
                .OrderByDescending(p => p.StockQuantity)
                .Take(3)
                .ToList();

            // Top 3 products with the highest price
            var topProductsByPrice = _context.Products
                .Where(p => !p.IsDeleted)  // Exclude deleted products
                .OrderByDescending(p => p.Price)
                .Take(3)
                .ToList();

            // Category with the most stock
            var topCategory = _context.Products
                .Where(p => !p.IsDeleted)  // Exclude deleted products
                .GroupBy(p => p.Category)
                .OrderByDescending(g => g.Sum(p => p.StockQuantity))
                .Select(g => new { Category = g.Key, TotalStock = g.Sum(p => p.StockQuantity) })
                .FirstOrDefault();

            // Pass the data to the view
            var viewModel = new DashboardViewModel
            {
                TotalProducts = totalProducts,
                TopProductsByQuantity = topProductsByQuantity,
                TopProductsByPrice = topProductsByPrice,
                TopCategory = topCategory?.Category
            };

            return View(viewModel);
        }
    }
}
