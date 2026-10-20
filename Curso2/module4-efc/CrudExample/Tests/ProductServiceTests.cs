using Xunit;
using Moq;
using CrudExample.Services;
using CrudExample.Repositories;
using CrudExample.Models;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _service = new ProductService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateProductAsync_ValidProduct_ReturnsProduct()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Teste description",
            Price = 99.99m
        };

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync(product);

        // Act
        var result = await _service.CreateProductAsync(product);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Product", result.Name);
        _mockRepository.Verify(r => r.CreateAsync(product), Times.Once);
    }

}