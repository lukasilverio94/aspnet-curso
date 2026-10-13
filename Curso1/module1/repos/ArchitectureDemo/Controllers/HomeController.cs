using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ArchitectureDemo.Models;

namespace ArchitectureDemo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /*
    render the index.cshtml view
    */
    public IActionResult Index()
    {
        return View();
    }

    /*
    render the privacy.cshtml view
    */
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
