using TechShop.Models;

namespace TechShop.ViewModels;

public class CartIndexViewModel
{
    public IReadOnlyList<CartItem> Items { get; init; } = [];
    public decimal Total { get; init; }
}
