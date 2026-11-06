using TechShop.Models;

namespace TechShop.Services;

/// <summary>Outcome of attempting to place an order from the session cart.</summary>
public sealed class CheckoutResult
{
    public bool Success { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];
    public Order? Order { get; init; }

    public static CheckoutResult Ok(Order order) => new() { Success = true, Order = order };

    public static CheckoutResult Fail(params string[] errors) =>
        new() { Success = false, Errors = errors };
}
