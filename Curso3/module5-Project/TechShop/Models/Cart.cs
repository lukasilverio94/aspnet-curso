namespace TechShop.Models;

/// <summary>
/// Shopping cart stored in session (serialized JSON).
/// </summary>
public class Cart
{
    public List<CartItem> Items { get; set; } = [];

    public decimal Total => Items.Sum(i => i.LineTotal);

    public decimal GetTotal()
    {
        return Total;
    }
}
