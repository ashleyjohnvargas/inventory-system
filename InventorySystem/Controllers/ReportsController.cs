using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InventorySystem.Models;

namespace InventorySystem.Controllers;

public class ReportsController : Controller
{
    private readonly ILogger<DashboardController> _logger;

    public ReportsController(ILogger<DashboardController> logger)
    {
        _logger = logger;
    }

    public IActionResult ReportsPage()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
