namespace WebApplicationFactoryDemo.Models
{
    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException(int id) : base($"Product with id {id} was not found.") { }
    }
}