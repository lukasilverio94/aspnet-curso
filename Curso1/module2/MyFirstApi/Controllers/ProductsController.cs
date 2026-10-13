using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ProductsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<string>> Get()
        {
            return new List<string> { "Appe", "Banana", "Orange" };
        }

        [HttpGet("featured")]
        public string GetFeaturedProduct() => "Mango";

        [HttpGet("{id}")]
        public ActionResult<string> GetProductById(int id)
        {
            var products = new List<string> { "Apple", "Banana", "Orange" };

            if (id < 0 || id >= products.Count)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            return products[id];
        }

        [HttpPost]
        public ActionResult<string> Post([FromBody] string newProduct)
        {
            return $"Added: {newProduct}";
        }

        [HttpPut("{id}")]
        public ActionResult<string> Put(int id, [FromBody] string updatedProduct)
        {
            return $"Updated product {id} to: {updatedProduct}";
        }

        [HttpDelete("{id}")]
        public ActionResult<string> Delete(int id)
        {
            return $"Deleted product with ID: {id}";
        }
    }

}