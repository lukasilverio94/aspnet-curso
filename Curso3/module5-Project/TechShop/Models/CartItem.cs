namespace TechShop.Models;

/// <summary>
/// One line in the cart. Product fields are snapshotted when added so the cart
/// stays consistent without loading navigation properties from JSON.
/// </summary>
public class CartItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    public decimal LineTotal => UnitPrice * Quantity;
}
