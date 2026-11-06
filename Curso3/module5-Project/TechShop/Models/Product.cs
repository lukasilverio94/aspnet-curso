using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TechShop.Models;

public class Product
{
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Plain-text marketing copy shown on product pages and in the cart snapshot.</summary>
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>Relative URL under wwwroot (e.g. /images/products/mouse.svg).</summary>
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    [Precision(18, 2)]
    [Range(0, 9999.99)]
    public decimal Price { get; set; }

    public int StockQuantity { get; set; } = 0;

    public bool IsAvailable { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public Category Category { get; set; } = null!;
}
