using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TechShop.ViewModels;

public class ProductFormViewModel
{
    public int? Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Image path or URL")]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    [Range(0, 99999.99)]
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Display(Name = "Stock quantity")]
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [Display(Name = "Available for sale")]
    public bool IsAvailable { get; set; }

    [Required]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [ValidateNever]
    public IEnumerable<SelectListItem> Categories { get; set; } = [];
}
