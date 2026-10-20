using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataManagement.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        // Foreign key
        public int CustomerId { get; set; }

        // Navigation property - many-to-one relationship
        public Customer Customer { get; set; } = null!;

        // Navigation property - one-to-many relationship
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}