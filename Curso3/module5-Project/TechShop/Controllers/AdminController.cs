using DataManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechShop.Models;
using TechShop.ViewModels;

namespace TechShop.Controllers;

[Authorize(Roles = IdentityDataSeeder.AdminRole)]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Dashboard()
    {
        var model = new AdminDashboardViewModel
        {
            TotalProducts = await _context.Products.CountAsync(),
            TotalOrders = await _context.Orders.CountAsync(),
            TotalUsers = await _userManager.Users.CountAsync(),
            RecentOrders = await _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync()
        };
        return View(model);
    }

    public async Task<IActionResult> ManageProducts()
    {
        var products = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();
        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> CreateProduct()
    {
        return View("ProductForm", await BuildProductFormAsync(null));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(ProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Categories = await GetCategorySelectListAsync();
            return View("ProductForm", model);
        }

        var product = new Product
        {
            Name = model.Name.Trim(),
            Description = model.Description.Trim(),
            ImageUrl = model.ImageUrl.Trim(),
            Price = model.Price,
            StockQuantity = model.StockQuantity,
            IsAvailable = model.IsAvailable,
            CategoryId = model.CategoryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        TempData["AdminMessage"] = "Product created.";
        return RedirectToAction(nameof(ManageProducts));
    }

    [HttpGet]
    public async Task<IActionResult> EditProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null)
            return NotFound();

        return View("ProductForm", await BuildProductFormAsync(product));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(ProductFormViewModel model)
    {
        if (!model.Id.HasValue)
            return BadRequest();

        var id = model.Id.Value;

        var product = await _context.Products.FindAsync(id);
        if (product is null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            model.Categories = await GetCategorySelectListAsync();
            return View("ProductForm", model);
        }

        product.Name = model.Name.Trim();
        product.Description = model.Description.Trim();
        product.ImageUrl = model.ImageUrl.Trim();
        product.Price = model.Price;
        product.StockQuantity = model.StockQuantity;
        product.IsAvailable = model.IsAvailable;
        product.CategoryId = model.CategoryId;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        TempData["AdminMessage"] = "Product updated.";
        return RedirectToAction(nameof(ManageProducts));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
        {
            TempData["AdminError"] = "Product not found.";
            return RedirectToAction(nameof(ManageProducts));
        }

        var hasOrderLines = await _context.OrderItems.AnyAsync(oi => oi.ProductId == id);
        if (hasOrderLines)
        {
            TempData["AdminError"] = $"Cannot delete \"{product.Name}\" because it appears on existing orders. Mark it unavailable instead.";
            return RedirectToAction(nameof(ManageProducts));
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        TempData["AdminMessage"] = "Product deleted.";
        return RedirectToAction(nameof(ManageProducts));
    }

    public async Task<IActionResult> ViewOrders()
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
        return View(orders);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrderStatus(int id, string status)
    {
        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Processing",
            "Shipped",
            "Completed",
            "Cancelled"
        };
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order is null)
            return NotFound();

        if (!allowed.Contains(status))
        {
            TempData["AdminError"] = "Invalid order status.";
            return RedirectToAction(nameof(ViewOrders));
        }

        order.Status = status;
        await _context.SaveChangesAsync();
        TempData["AdminMessage"] = $"Order #{id} status set to {status}.";
        return RedirectToAction(nameof(ViewOrders));
    }

    public async Task<IActionResult> Users()
    {
        var usersList = await _userManager.Users
            .AsNoTracking()
            .OrderBy(u => u.Email)
            .ToListAsync();

        var vm = new List<AdminUserListViewModel>();
        foreach (var u in usersList)
        {
            var roles = await _userManager.GetRolesAsync(u);
            var lockoutEnd = await _userManager.GetLockoutEndDateAsync(u);
            var isLocked = lockoutEnd.HasValue && lockoutEnd.Value > DateTimeOffset.UtcNow;
            vm.Add(new AdminUserListViewModel
            {
                UserId = u.Id,
                Email = u.Email ?? "",
                FirstName = u.FirstName,
                LastName = u.LastName,
                EmailConfirmed = u.EmailConfirmed,
                IsLockedOut = isLocked,
                Roles = roles.OrderBy(r => r).ToList()
            });
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLockUser(string userId, string enableLockout)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound();

        if (user.Id == _userManager.GetUserId(User))
        {
            TempData["AdminError"] = "You cannot lock or unlock your own account.";
            return RedirectToAction(nameof(Users));
        }

        var lockAccount = string.Equals(enableLockout, "true", StringComparison.OrdinalIgnoreCase);
        await _userManager.SetLockoutEnabledAsync(user, true);
        if (lockAccount)
        {
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
            TempData["AdminMessage"] = $"User {user.Email} has been locked.";
        }
        else
        {
            await _userManager.SetLockoutEndDateAsync(user, null);
            TempData["AdminMessage"] = $"User {user.Email} has been unlocked.";
        }

        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetUserRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound();

        if (user.Id == _userManager.GetUserId(User))
        {
            TempData["AdminError"] = "You cannot change your own role.";
            return RedirectToAction(nameof(Users));
        }

        var validRoles = new[] { IdentityDataSeeder.CustomerRole, IdentityDataSeeder.AdminRole };
        if (!validRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
        {
            TempData["AdminError"] = "Invalid role.";
            return RedirectToAction(nameof(Users));
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Count > 0)
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, role);

        TempData["AdminMessage"] = $"Role for {user.Email} set to {role}.";
        return RedirectToAction(nameof(Users));
    }

    private async Task<IEnumerable<SelectListItem>> GetCategorySelectListAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
            .ToListAsync();
    }

    private async Task<ProductFormViewModel> BuildProductFormAsync(Product? product)
    {
        var categories = await GetCategorySelectListAsync();
        if (product is null)
        {
            return new ProductFormViewModel
            {
                Categories = categories,
                IsAvailable = true
            };
        }

        return new ProductFormViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            IsAvailable = product.IsAvailable,
            CategoryId = product.CategoryId,
            Categories = categories
        };
    }
}
