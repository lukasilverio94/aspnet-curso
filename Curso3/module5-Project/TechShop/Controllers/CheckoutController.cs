using DataManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechShop.Models;
using TechShop.Services;
using TechShop.ViewModels;

namespace TechShop.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICheckoutService _checkoutService;

    public CheckoutController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ICheckoutService checkoutService)
    {
        _context = context;
        _userManager = userManager;
        _checkoutService = checkoutService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var cart = CartSessionHelper.GetCart(HttpContext.Session);
        if (cart.Items.Count == 0)
            return RedirectToAction(nameof(CartController.Index), "Cart");

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Challenge();

        var model = new CheckoutViewModel
        {
            Cart = cart,
            Email = user.Email ?? User.Identity?.Name ?? string.Empty,
            ShippingAddress = string.Empty,
            PaymentMethod = string.Empty
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessOrder(CheckoutViewModel model, CancellationToken cancellationToken)
    {
        var cart = CartSessionHelper.GetCart(HttpContext.Session);
        model.Cart = cart;

        if (cart.Items.Count == 0)
            return RedirectToAction(nameof(CartController.Index), "Cart");

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Challenge();

        if (!string.Equals(model.Email?.Trim(), user.Email, StringComparison.OrdinalIgnoreCase))
            ModelState.AddModelError(nameof(model.Email), "Email must match your account email.");

        if (!ModelState.IsValid)
            return View("Index", model);

        var result = await _checkoutService.PlaceOrderAsync(
            user.Id,
            cart,
            model.ShippingAddress,
            model.PaymentMethod,
            cancellationToken);

        if (!result.Success)
        {
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err);
            return View("Index", model);
        }

        CartSessionHelper.ClearCart(HttpContext.Session);

        return RedirectToAction(nameof(OrderConfirmation), new { orderId = result.Order!.Id });
    }

    [HttpGet]
    public async Task<IActionResult> OrderConfirmation(int orderId, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Challenge();

        var order = await _context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order is null)
            return NotFound();

        if (order.UserId != user.Id)
            return Forbid();

        return View(new OrderConfirmationViewModel { Order = order });
    }
}
