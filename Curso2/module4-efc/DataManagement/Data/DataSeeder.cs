using DataManagement.Data;
using DataManagement.Models;

namespace DataManagement.Data
{
    public class DataSeeder
    {
        public static void SeedData(ApplicationDbContext context)
        {
            // Clear existing data
            context.Database.EnsureCreated();

            if (context.Customers.Any())
            {
                return; // Database already seeded
            }

            // Create customers
            var customer1 = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "555-0100"
            };

            var customer2 = new Customer
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Phone = "555-0200"
            };

            // Create categories
            var electronics = new Category
            {
                Name = "Electronics",
                Description = "Electronic devices and accessories"
            };

            var books = new Category
            {
                Name = "Books",
                Description = "Physical and digital books"
            };

            // Create products
            var laptop = new Product
            {
                Name = "Laptop",
                Description = "High-performance laptop",
                Price = 999.99m,
                StockQuantity = 50,
                Categories = new List<Category> { electronics }
            };

            var novel = new Product
            {
                Name = "Programming Guide",
                Description = "Comprehensive programming guide",
                Price = 49.99m,
                StockQuantity = 100,
                Categories = new List<Category> { books }
            };

            // Create orders
            var order1 = new Order
            {
                OrderDate = DateTime.Now,
                Status = "Processing",
                Customer = customer1,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Product = laptop,
                        Quantity = 1,
                        UnitPrice = laptop.Price
                    }
                }
            };

            order1.TotalAmount = order1.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice);

            // Add to context
            context.Customers.AddRange(customer1, customer2);
            context.Categories.AddRange(electronics, books);
            context.Products.AddRange(laptop, novel);
            context.Orders.Add(order1);

            // Save changes
            context.SaveChanges();
        }
    }
}