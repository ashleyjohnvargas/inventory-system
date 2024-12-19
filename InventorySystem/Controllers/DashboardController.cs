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

            // Total number of out-of-stock products (either IsDeleted is true OR StockQuantity is 0)
            var outOfStockProducts = _context.Products.Count(p => !p.IsDeleted && p.CurrentStock == 0);

            // Top 3 products with the highest quantity
            var topProductsByQuantity = _context.Products
                .Where(p => !p.IsDeleted)  // Exclude deleted products
                .OrderByDescending(p => p.CurrentStock)
                .Take(5)
                .ToList();

            // Top 3 products with the highest price
            var topProductsByPrice = _context.Products
                .Where(p => !p.IsDeleted)  // Exclude deleted products
                .OrderByDescending(p => p.Price)
                .Take(5)
                .ToList();

            // Top 8 categories with the most stock
            var topCategoriesByStock = _context.Products
                .Where(p => !p.IsDeleted)  // Exclude deleted products
                .GroupBy(p => p.Category)
                .OrderByDescending(g => g.Sum(p => p.CurrentStock))
                .Take(8)
                .Select(g => new CategoryStock
                {
                    CategoryName = g.Key,
                    StockQuantity = g.Sum(p => p.CurrentStock)
                })
                .ToList();

            // Calculate product additions based on DateAdded
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfYear = new DateTime(today.Year, 1, 1);

            var productsAddedToday = _context.Products.Count(p => !p.IsDeleted && p.DateAdded.Date == today);
            var productsAddedThisWeek = _context.Products.Count(p => !p.IsDeleted && p.DateAdded.Date >= startOfWeek);
            var productsAddedThisMonth = _context.Products.Count(p => !p.IsDeleted && p.DateAdded.Date >= startOfMonth);
            var productsAddedThisYear = _context.Products.Count(p => !p.IsDeleted && p.DateAdded.Date >= startOfYear);


            // Pass the data to the view
            var viewModel = new DashboardViewModel
            {
                TotalProducts = totalProducts,
                OutOfStockProducts = outOfStockProducts,
                TopProductsByQuantity = topProductsByQuantity,
                TopProductsByPrice = topProductsByPrice,
                TopCategoriesByStock = topCategoriesByStock,  // Correct assignment to TopCategoriesByStock
                ProductsAddedToday = productsAddedToday,
                ProductsAddedThisWeek = productsAddedThisWeek,
                ProductsAddedThisMonth = productsAddedThisMonth,
                ProductsAddedThisYear = productsAddedThisYear
            };

            return View(viewModel);
        }

    }
}
