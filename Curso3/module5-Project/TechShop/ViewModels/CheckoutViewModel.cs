using System.ComponentModel.DataAnnotations;
using TechShop.Models;

namespace TechShop.ViewModels;

public class CheckoutViewModel
{
    public Cart Cart { get; set; } = new();

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Shipping address is required.")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Address must be between 10 and 500 characters.")]
    [Display(Name = "Shipping address")]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Select a payment method.")]
    [RegularExpression("^(Card|PayPal)$", ErrorMessage = "Select a valid payment method.")]
    [Display(Name = "Payment method")]
    public string PaymentMethod { get; set; } = string.Empty;
}
