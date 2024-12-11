using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InventorySystem.Models;

namespace InventorySystem.Controllers;

public class StockController : Controller
{
    private readonly ILogger<DashboardController> _logger;

    public StockController(ILogger<DashboardController> logger)
    {
        _logger = logger;
    }

    public IActionResult StockLevels()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
