using System.ComponentModel.DataAnnotations;

namespace DataManagement.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        // Navigation property - many-to-many relationship
        public ICollection<Product> Products { get; set; } = new List<Product>();

    }
}