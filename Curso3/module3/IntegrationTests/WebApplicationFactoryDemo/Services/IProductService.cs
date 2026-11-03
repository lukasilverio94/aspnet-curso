// Services/IProductService.cs
using WebApplicationFactoryDemo.Models;

namespace WebApplicationFactoryDemo.Services;

public interface IProductService
{
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default);
    Task<Product> CreateAsync(Product product, CancellationToken ct = default);
}
