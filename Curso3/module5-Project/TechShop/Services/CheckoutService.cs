using DataManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TechShop.Models;

namespace TechShop.Services;

public class CheckoutService : ICheckoutService
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CheckoutService> _logger;

    public CheckoutService(
        ApplicationDbContext db,
        IConfiguration configuration,
        ILogger<CheckoutService> logger)
    {
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<CheckoutResult> PlaceOrderAsync(
        string userId,
        Cart cart,
        string shippingAddress,
        string paymentMethod,
        CancellationToken cancellationToken = default)
    {
        if (cart.Items.Count == 0)
            return CheckoutResult.Fail("Your cart is empty.");

        var maxOrder = _configuration.GetValue("Checkout:MaxOrderTotal", 10_000m);
        if (maxOrder > 0 && cart.Total > maxOrder)
        {
            return CheckoutResult.Fail(
                $"For security, orders over {maxOrder:C} cannot be placed online. Please split your order or contact support.");
        }

        var productIds = cart.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        var errors = new List<string>();
        decimal computedTotal = 0;

        foreach (var line in cart.Items)
        {
            if (!products.TryGetValue(line.ProductId, out var product))
            {
                errors.Add($"Product #{line.ProductId} is no longer available.");
                break;
            }

            if (!product.IsAvailable)
            {
                errors.Add($"{product.Name} is not available.");
                break;
            }

            if (line.Quantity <= 0 || line.Quantity > product.StockQuantity)
            {
                errors.Add($"Invalid quantity for {product.Name} (stock: {product.StockQuantity}).");
                break;
            }

            computedTotal += product.Price * line.Quantity;
        }

        if (errors.Count > 0)
            return CheckoutResult.Fail(errors.ToArray());

        if (Math.Abs(computedTotal - cart.Total) > 0.02m)
        {
            _logger.LogWarning(
                "Cart total mismatch for user {UserId}: session {SessionTotal} vs computed {Computed}",
                userId, cart.Total, computedTotal);
            return CheckoutResult.Fail("Your cart prices are out of date. Please review your cart and try again.");
        }

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        Order? createdOrder = null;

        try
        {
            createdOrder = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Total = computedTotal,
                ShippingAddress = shippingAddress.Trim(),
                PaymentMethod = paymentMethod.Trim(),
                Status = "Processing",
                PaymentStatus = "Pending"
            };

            _db.Orders.Add(createdOrder);
            await _db.SaveChangesAsync(cancellationToken);

            foreach (var line in cart.Items)
            {
                var product = products[line.ProductId];
                var unitPrice = product.Price;

                _db.OrderItems.Add(new OrderItem
                {
                    OrderId = createdOrder.Id,
                    ProductId = product.Id,
                    Quantity = line.Quantity,
                    UnitPrice = unitPrice
                });

                product.StockQuantity -= line.Quantity;
                product.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Checkout failed for user {UserId}", userId);
            return CheckoutResult.Fail("We could not complete your order. Please try again.");
        }

        if (createdOrder is null)
            return CheckoutResult.Fail("We could not complete your order. Please try again.");

        await SimulatePaymentProcessingAsync(createdOrder, cancellationToken);

        return CheckoutResult.Ok(createdOrder);
    }

    private async Task SimulatePaymentProcessingAsync(Order order, CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
        order.Status = "Completed";
        order.PaymentStatus = "Paid";
        await _db.SaveChangesAsync(cancellationToken);
    }
}
