using CrudExample.Models;
using CrudExample.Repositories;
using CrudExample.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        // validate product name
        if (string.IsNullOrWhiteSpace(product.Name))
        {
            throw new ArgumentException("Product name cannot be empty", nameof(product.Name));
        }

        // validate price
        if (product.Price <= 0)
        {
            throw new ArgumentException("Product price must be positive", nameof(product.Price));
        }

        // Call the repository to create the Product
        return await _repository.CreateAsync(product);
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        // Validate ID
        if (id <= 0)
        {
            throw new ArgumentException("Product ID must be positive", nameof(id));
        }
        // Call repository to retrieve the product
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        // Call Repository to retrive all products
        return await _repository.GetAllAsync();
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        if (product.Id <= 0)
        {
            throw new ArgumentException("Product ID must be positive", nameof(product.Id));
        }

        if (string.IsNullOrWhiteSpace(product.Name))
        {
            throw new ArgumentException("Product name cannot be empty", nameof(product.Name));
        }
        if (string.IsNullOrWhiteSpace(product.Description))
        {
            throw new ArgumentException("Product description cannot be empty", nameof(product.Description));
        }
        if (product.Price <= 0)
        {
            throw new ArgumentException("Product price must be positive", nameof(product.Price));
        }
        return await _repository.UpdateAsync(product);
    }


    public async Task DeleteProductAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Product ID must be positive", nameof(id));
        }

        await _repository.DeleteAsync(id);
    }
}