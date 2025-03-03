using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InventorySystem.Models;
using InventorySystem.Services;

namespace InventorySystem.Controllers
{
    // [Route("api/[controller]")] is declared below in order to specify that this controller, which is the ProductsController is also
    // used for managing or controlling methods/actions for API routes. 
    // [Route("api/[controller]")]
    // RULE: Don't create a web API controller by deriving from the Controller class. Controller derives from ControllerBase and adds support 
    // for views, so it's for handling web pages, not web API requests. 
    // There's an exception to this rule: if you plan to use the same controller for both views and web APIs, derive it from Controller.
    // Interpretation: The ProductsController inherited from Controller instead of ControllerBase because this controller is used
    // both for (1) MVC Routes or MVC Controller Actions and for (2) API Routes or API Endpoints.
    // The term used for all actions or methods here that are not used for API purposes but exclusively for this system (InventorySystem)
    // is MVC Routes. While the action or method in this controller which is used for API purposes is called API Routes or API Endpoints.
    // This controller exposes API endpoint which is the GetAllProducts action as it has an indicated route which is [HttpGet("GetAllProducts")].
    // MVC Controller Actions also have routing here such as [HttpGet("/ProductsPage")] that's why they're called MVC Routes.
    // The MVC Controller Actions in this controller WOULD NOT have routing such as [HttpGet("/ProductsPage")] if this Controller
    // does not contain API Routes or API Endpoints such as the GetAllProducts action/method. But since this controller contains 
    // API Routes, then each of the MVC Controller Actions must be routed as well because as you can see above, this is declared: [Route("api/[controller]")].
    // That requires this controller to have API routing such as HttpPost, HttpGet, and such, for each action here, whether an MVC Action
    // or API action such as the GetAllProducts action. But if this controller DOES NOT contain API routes such as GetAllProducts, you
    // don't need to declare this in this Controller: [Route("api/[controller]")], since this controller doesn't contain API Endpoiint,
    // or an action/method for API purposes. And without that, you also will not need to provide ROUTING for each MVC Action here such
    // as this one: [HttpGet("/ProductsPage")]. So if you don't want to put routing for each MVC Action in this controller,
    // remove the API Endpoint or action here which is the GetAllProducts()
    // But as you can see, I removed the API Endpoint here and transferred to Api folder in the ProductsApiController.cs
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly EcommerceService _ecommerceService;

        public ProductsController(ILogger<ProductsController> logger, ApplicationDbContext context, EcommerceService ecommerceService)
        {
            _logger = logger;
            _context = context;
            _ecommerceService = ecommerceService;
        }

        // MVC Route: ProductsPage
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
        public IActionResult AddProductPage()
        {
            return View();
        }

        // POST: AddProduct
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                
                // Calculate StockStatus based on CurrentStock and OriginalStock
                if (product.CurrentStock == 0)
                {
                    product.StockStatus = "Out-of-Stock";
                }
                else if (product.CurrentStock < (0.1 * product.OriginalStock))
                {
                    product.StockStatus = "Low Stock";
                }
                else
                {
                    product.StockStatus = "In Stock";
                }

                try
                {
                    // Add product to the database
                    _context.Products.Add(product);
                    _context.SaveChanges();

                    // Redirect to the products page with success notification
                    return RedirectToAction("ProductsPage", new { page = 1 });
                }
                catch (Exception ex)
                {
                    // Log the error and display an error message to the user
                    _logger.LogError(ex, "Error occurred while adding a product.");
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the product. Please try again.");
                }
            }

            // If we got this far, something failed, redisplay the form
            Console.WriteLine("Error adding product!");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View("AddProductPage", product);
        }


        // GET: EditProduct
        public IActionResult EditProductPage(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: AddProduct
        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                // Calculate StockStatus based on CurrentStock and OriginalStock
                if (product.CurrentStock == 0)
                {
                    product.StockStatus = "Out-of-Stock";
                    product.IsBeingSold = false;

                }
                else if (product.CurrentStock < (0.1 * product.OriginalStock))
                {
                    product.StockStatus = "Low Stock";
                    product.IsBeingSold = true;
                }
                else
                {
                    product.StockStatus = "In Stock";
                    product.IsBeingSold = true;
                }

                try
                {
                    // Add product to the database
                    _context.Products.Update(product);
                    // Save the changes to the database
                    await _context.SaveChangesAsync();

                    // Update the product details in the Ecommerce System
                    await _ecommerceService.UpdateProductInEcommerceSystem(product);

                    Console.WriteLine(product.Id);

                    // Redirect to the products page with success notification
                    TempData["SuccessMessage"] = "Product updated successfully.";
                    return RedirectToAction("ProductsPage", new { page = 1 });
                }
                catch (Exception ex)
                {
                    // Log the error and display an error message to the user
                    _logger.LogError(ex, "Error occurred while adding a product.");
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the product. Please try again.");
                }
            }
            // If we got this far, something failed, redisplay the form
            Console.WriteLine("Error editing product!");
            return View("EditProductPage", product);
        }

        // POST: DeleteProduct
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                product.IsDeleted = true; // Mark the product as deleted
                product.IsBeingSold = false; // Mark the product as no longer being sold
                product.StockStatus = "Discontinued"; // Update stock status to Discontinued

                try
                {
                    _context.Products.Update(product);
                    _context.SaveChanges();

                    // Set a success message using TempData
                    TempData["SuccessMessage"] = "Product deleted successfully.";
                }
                catch (Exception ex)
                {
                    // Log the error and set an error message using TempData
                    _logger.LogError(ex, "Error occurred while deleting the product.");
                    TempData["ErrorMessage"] = "An error occurred while deleting the product. Please try again.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Product not found.";
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
