using TechShop.Models;

namespace TechShop.Services;

public interface ICheckoutService
{
    /// <summary>
    /// Validates the cart against the database, creates the order in a transaction,
    /// decrements stock, then simulates payment (delayed status update).
    /// Does not clear session — the caller does that after success.
    /// </summary>
    Task<CheckoutResult> PlaceOrderAsync(
        string userId,
        Cart cart,
        string shippingAddress,
        string paymentMethod,
        CancellationToken cancellationToken = default);
}
