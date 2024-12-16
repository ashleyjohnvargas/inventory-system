using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InventorySystem.Models;

namespace InventorySystem.Controllers.Api 
{
    [Route("api/[controller]/[action]")]
    [ApiController] // Specifies that this is an API controller
    public class ProductsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        //[HttpGet("GetAllProducts")] this is unnecessary anymore since in [Route("api/[controller]/[action]")], the action is already
        // included in the routing. Meaning, the name of the action will be put in the action syntax above. But you can remove that
        // just like this: [Route("api/[controller]")]. But since there's no action, you'll need to include this: [HttpGet("GetAllProducts")]
        // above this action name below:
        public IActionResult GetAllProducts()
        {
            var products = _context.Products
                                    .Where(p => !p.IsDeleted)
                                    .ToList();
            return Ok(products);
        }
    }
}