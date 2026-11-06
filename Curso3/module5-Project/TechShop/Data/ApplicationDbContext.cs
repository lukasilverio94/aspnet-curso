using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechShop.Models;

namespace DataManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(e =>
            {
                e.Property(o => o.Total).HasPrecision(18, 2);

                e.HasOne(o => o.User)
                    .WithMany()
                    .HasForeignKey(o => o.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OrderItem>(e =>
            {
                e.HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(oi => oi.Product)
                    .WithMany()
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.Property(oi => oi.UnitPrice).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .IsRequired();

            // -- Seed categories --
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Books" },
                new Category { Id = 3, Name = "Home & Kitchen" }
            );

            // -- Seed products --
            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Wireless Mouse",
                    Description = "Ergonomic wireless mouse with precision tracking and long battery life.",
                    ImageUrl = "/images/products/mouse.svg",
                    Price = 29.99m,
                    StockQuantity = 100,
                    IsAvailable = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "Mechanical Keyboard",
                    Description = "Tactile switches, RGB backlight, and durable PBT keycaps for daily coding.",
                    ImageUrl = "/images/products/keyboard.svg",
                    Price = 89.99m,
                    StockQuantity = 50,
                    IsAvailable = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 3,
                    Name = "Clean Code",
                    Description = "A handbook of agile software craftsmanship by Robert C. Martin.",
                    ImageUrl = "/images/products/book.svg",
                    Price = 39.99m,
                    StockQuantity = 20,
                    IsAvailable = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate,
                    CategoryId = 2
                },
                new Product
                {
                    Id = 4,
                    Name = "Stainless Steel Pan",
                    Description = "12-inch tri-ply stainless skillet. Currently out of stock.",
                    ImageUrl = "/images/products/pan.svg",
                    Price = 24.50m,
                    StockQuantity = 0,
                    IsAvailable = false,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate,
                    CategoryId = 3
                }
            );
        }
    }
}
