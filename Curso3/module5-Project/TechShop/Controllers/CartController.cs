using DataManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechShop.Models;
using TechShop.Services;
using TechShop.ViewModels;

namespace TechShop.Controllers;

public class CartController : Controller
{
    public const string SessionKey = "Cart";

    private readonly ApplicationDbContext _db;

    public CartController(ApplicationDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        var cart = CartSessionHelper.GetCart(HttpContext.Session);
        var vm = new CartIndexViewModel
        {
            Items = cart.Items,
            Total = cart.Total
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1, string? returnUrl = null)
    {
        var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == productId);
        if (product is null)
            return NotFound();

        if (!product.IsAvailable || product.StockQuantity <= 0)
        {
            TempData["CartMessage"] = "This product is not available.";
            return returnUrl is not null && Url.IsLocalUrl(returnUrl)
                ? LocalRedirect(returnUrl)
                : RedirectToAction(nameof(ProductController.Details), "Product", new { id = productId });
        }

        quantity = Math.Clamp(quantity, 1, product.StockQuantity);
        var cart = CartSessionHelper.GetCart(HttpContext.Session);
        var existing = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
        {
            var newQty = Math.Min(existing.Quantity + quantity, product.StockQuantity);
            existing.Quantity = newQty;
            existing.UnitPrice = product.Price;
            existing.ProductName = product.Name;
            existing.Description = product.Description;
            existing.ImageUrl = product.ImageUrl;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                UnitPrice = product.Price,
                Quantity = quantity
            });
        }

        CartSessionHelper.SaveCart(HttpContext.Session, cart);
        TempData["CartMessage"] = "Added to cart.";

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);
        return RedirectToAction(nameof(ProductController.Details), "Product", new { id = productId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int productId, int quantity)
    {
        var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == productId);
        if (product is null)
            return NotFound();

        var cart = CartSessionHelper.GetCart(HttpContext.Session);
        var line = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (line is null)
            return RedirectToAction(nameof(Index));

        if (quantity <= 0 || !product.IsAvailable)
        {
            cart.Items.Remove(line);
            CartSessionHelper.SaveCart(HttpContext.Session, cart);
            return RedirectToAction(nameof(Index));
        }

        line.Quantity = Math.Min(quantity, product.StockQuantity);
        line.UnitPrice = product.Price;
        CartSessionHelper.SaveCart(HttpContext.Session, cart);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int productId)
    {
        var cart = CartSessionHelper.GetCart(HttpContext.Session);
        var line = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (line is not null)
        {
            cart.Items.Remove(line);
            CartSessionHelper.SaveCart(HttpContext.Session, cart);
        }

        return RedirectToAction(nameof(Index));
    }

}
