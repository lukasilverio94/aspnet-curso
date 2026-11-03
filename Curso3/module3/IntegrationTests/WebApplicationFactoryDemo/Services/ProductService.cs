using WebApplicationFactoryDemo.Models;

namespace WebApplicationFactoryDemo.Services;

public class ProductService : IProductService
{
    // In-memory store; in a real app this would be a DbContext or repository.
    private static readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Keyboard", Price = 79.99m, Stock = 10 },
        new Product { Id = 2, Name = "Mouse",    Price = 39.99m, Stock = 25 },
    };
    private static int _nextId = 3;
    private static readonly object _lock = new();

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
        => Task.FromResult(_products.FirstOrDefault(p => p.Id == id));

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Product>>(_products.ToList());

    public Task<Product> CreateAsync(Product product, CancellationToken ct = default)
    {
        lock (_lock)
        {
            product.Id = _nextId++;
            _products.Add(product);
            return Task.FromResult(product);
        }
    }
}