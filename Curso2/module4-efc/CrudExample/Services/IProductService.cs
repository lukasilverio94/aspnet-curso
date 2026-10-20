using System;
using CrudExample.Models;

namespace CrudExample.Services;

public interface IProductService
{
    Task<Product?> GetProductAsync(int id);
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product> CreateProductAsync(Product product);
    Task<Product> UpdateProductAsync(Product product);
    Task DeleteProductAsync(int id);
}
