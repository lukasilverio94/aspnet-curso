using System.Diagnostics.CodeAnalysis;
using DataManagement.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TechShop.Controllers;
using TechShop.Models;
using TechShop.Services;

namespace TechShop.Tests;

/// <summary>
/// Unit tests: CartController + in-memory EF + in-memory session on DefaultHttpContext.
/// No HTTP server — antiforgery and filters are not executed.
/// </summary>
public class CartControllerUnitTests
{
    [Fact]
    public async Task AddToCart_ProductExists_AddsLineAndSavesSession()
    {
        await using var db = await CreateDbWithProductAsync();
        var httpContext = CreateHttpContextWithSession();
        AttachTempData(httpContext);

        var controller = new CartController(db)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            Url = CreateUrlHelper()
        };

        var result = await controller.AddToCart(productId: 1, quantity: 2);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ProductController.Details), redirect.ActionName);

        var cart = CartSessionHelper.GetCart(httpContext.Session);
        Assert.Single(cart.Items);
        Assert.Equal(1, cart.Items[0].ProductId);
        Assert.Equal(2, cart.Items[0].Quantity);
    }

    [Fact]
    public async Task AddToCart_ProductMissing_ReturnsNotFound()
    {
        await using var db = await CreateEmptyDbAsync();
        var httpContext = CreateHttpContextWithSession();
        AttachTempData(httpContext);

        var controller = new CartController(db)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            Url = CreateUrlHelper()
        };

        var result = await controller.AddToCart(productId: 999, quantity: 1);

        Assert.IsType<NotFoundResult>(result);
    }

    private static async Task<ApplicationDbContext> CreateDbWithProductAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new ApplicationDbContext(options);

        var category = new Category { Name = "Electronics" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var product = new Product
        {
            Name = "Test Mouse",
            Description = "Desc",
            ImageUrl = "/images/x.svg",
            Price = 19.99m,
            StockQuantity = 10,
            IsAvailable = true,
            CategoryId = category.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.Products.Add(product);
        await db.SaveChangesAsync();

        return db;
    }

    private static async Task<ApplicationDbContext> CreateEmptyDbAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new ApplicationDbContext(options);
        await db.Database.EnsureCreatedAsync();
        return db;
    }

    private static DefaultHttpContext CreateHttpContextWithSession()
    {
        var httpContext = new DefaultHttpContext();
        var services = new ServiceCollection();
        services.AddSingleton<ITempDataDictionaryFactory, TestTempDataDictionaryFactory>();
        httpContext.RequestServices = services.BuildServiceProvider();
        httpContext.Features.Set<ISessionFeature>(new FakeSessionFeature(new MemorySession()));
        return httpContext;
    }

    private static void AttachTempData(HttpContext httpContext)
    {
        // AddToCart sets TempData — Controller.TempData uses ITempDataDictionaryFactory.GetTempData.
        var provider = new Mock<ITempDataProvider>();
        provider.Setup(p => p.LoadTempData(It.IsAny<HttpContext>()))
            .Returns(new Dictionary<string, object>(StringComparer.Ordinal));
        provider.Setup(p => p.SaveTempData(It.IsAny<HttpContext>(), It.IsAny<IDictionary<string, object>>()));

        var tempData = new TempDataDictionary(httpContext, provider.Object);
        httpContext.Features.Set<ITempDataDictionary>(tempData);
    }

    private static IUrlHelper CreateUrlHelper()
    {
        var url = new Mock<IUrlHelper>();
        url.Setup(u => u.IsLocalUrl(It.IsAny<string>())).Returns(false);
        return url.Object;
    }
}

file sealed class TestTempDataDictionaryFactory : ITempDataDictionaryFactory
{
    public ITempDataDictionary GetTempData(HttpContext context) =>
        context.Features.Get<ITempDataDictionary>()
        ?? throw new InvalidOperationException("Call AttachTempData before using the controller.");
}

file sealed class FakeSessionFeature(ISession session) : ISessionFeature
{
    public ISession Session { get; set; } = session;
}

/// <summary>Minimal in-memory ISession for unit tests (GetString/SetString).</summary>
internal sealed class MemorySession : ISession
{
    private readonly Dictionary<string, byte[]> _store = new(StringComparer.Ordinal);

    public bool IsAvailable => true;
    public string Id { get; } = "unit-test-session";
    public IEnumerable<string> Keys => _store.Keys;

    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public bool TryGetValue(string key, [NotNullWhen(true)] out byte[]? value)
    {
        return _store.TryGetValue(key, out value);
    }

    public void Set(string key, byte[] value) => _store[key] = value;

    public void Remove(string key) => _store.Remove(key);

    public void Clear() => _store.Clear();
}
