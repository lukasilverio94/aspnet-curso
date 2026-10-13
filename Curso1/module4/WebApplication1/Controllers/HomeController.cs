using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        ViewBag.UserName = "John Doen";
        ViewBag.UserSettings = new { Theme = "dark", Language = "en" };
        return View();
    }

    [HttpGet]
    public IActionResult GetUserData()
    {
        var data = new
        {
            Name = "Jane Smith",
            Email = "jane@example.com",
            LastLogin = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
        return Json(data);
    }

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
