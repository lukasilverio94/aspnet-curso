using System.ComponentModel.DataAnnotations;
namespace DataManagement.Models;

public class Customer
{

    public int Id { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(15)]
    public string? Phone { get; set; }

    // Navigation property - one-to-many relationship
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
