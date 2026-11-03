// Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using WebApplicationFactoryDemo.Models;
using WebApplicationFactoryDemo.Services;

namespace WebApplicationFactoryDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id, CancellationToken ct)
    {
        var product = await _service.GetByIdAsync(id, ct);
        if (product is null)
            throw new ProductNotFoundException(id);   // <-- downstream exception
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create([FromBody] Product product, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException("Product name is required.", nameof(product));
        var created = await _service.CreateAsync(product, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // Deliberately failing endpoint for testing exception propagation:
    [HttpGet("boom")]
    public IActionResult Boom() => throw new InvalidOperationException("Simulated downstream failure");
}