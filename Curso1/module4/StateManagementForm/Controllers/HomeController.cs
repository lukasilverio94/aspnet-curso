using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StateManagementForm.Models;

namespace StateManagementForm.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Form()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ProcessForm(string? userInput)
    {
        if (!string.IsNullOrWhiteSpace(userInput))
        {
            TempData["SuccessMessage"] = $"Successfully processed: {userInput}";
            TempData["MessageType"] = "success";
        }
        else
        {
            TempData["ErrorMessage"] = "Input cannot be empty";
            TempData["MessageType"] = "error";
        }
        return RedirectToAction(nameof(Form));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SavePreferences(string theme, string language)
    {
        HttpContext.Session.SetString("UserTheme", theme);
        HttpContext.Session.SetString("UserLanguage", language);

        TempData["SuccessMessage"] = "Preferences saved successfully!";
        return RedirectToAction("Preferences");
    }

    public IActionResult Preferences()
    {
        ViewBag.CurrentTheme = HttpContext.Session.GetString("UserTheme") ?? "light";
        ViewBag.CurrentLanguage = HttpContext.Session.GetString("UserLanguage") ?? "pt-BR";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
