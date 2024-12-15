using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InventorySystem.Models;

namespace InventorySystem.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller // Don't create a web API controller by deriving from the Controller class. Controller derives from ControllerBase and adds support for views, so it's for handling web pages, not web API requests. There's an exception to this rule: if you plan to use the same controller for both views and web APIs, derive it from Controller.
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly ApplicationDbContext _context;

        public ProductsController(ILogger<ProductsController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // MVC Route: ProductsPage
        [HttpGet("/ProductsPage")]
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


        // MVC Route: AddProduct
        [HttpGet("/AddProduct")]
        public IActionResult AddProduct()
        {
            return View();
        }

        // POST: AddProduct
        [HttpPost("/AddProduct")]
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

        // POST: AddProduct
        [HttpPost("/EditProduct")]
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
        // POST: AddProduct
        [HttpPost("/DeleteProduct")]
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

        // API Endpoint: GetAllProducts
        [HttpGet("GetAllProducts")]
        public IActionResult GetAllProducts()
        {
            var products = _context.Products
                           .Where(p => !p.IsDeleted) // Exclude soft-deleted products
                           .ToList(); 
            return Ok(products);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
