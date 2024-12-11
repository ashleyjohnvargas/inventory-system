using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InventorySystem.Models;

namespace InventorySystem.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly ApplicationDbContext _context;

        public ProductsController(ILogger<ProductsController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

       // GET: ProductsPage
        public IActionResult ProductsPage(int page = 1)
        {
            int pageSize = 10;  // Number of products per page
            var totalProducts = _context.Products.Count(p => !p.IsDeleted);  // Exclude soft-deleted products
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);  // Calculate total pages

            // Fetch the products for the current page
            var products = _context.Products
                                    .Where(p => !p.IsDeleted)  // Exclude soft-deleted products
                                    .Skip((page - 1) * pageSize)  // Skip products from previous pages
                                    .Take(pageSize)  // Take 10 products for the current page
                                    .ToList();

            var model = new PaginatedProductModel
            {
                Products = products,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(model);
        }


        // GET: AddProduct
        public IActionResult AddProduct()
        {
            return View();
        }

        // POST: AddProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("ProductsPage");
            }
            return View("ProductsPage", _context.Products.ToList());
        }

        // GET: EditProduct
        public IActionResult EditProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: EditProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Update(product);
                _context.SaveChanges();
                return RedirectToAction("ProductsPage");
            }
            return View(product);
        }

        // POST: DeleteProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                product.IsDeleted = true; // Mark the product as deleted
                _context.Products.Update(product);
                _context.SaveChanges();
            }
            return RedirectToAction("ProductsPage");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
