var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger Services
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

/* 
Product Minimum API Endpoints
No Controllers Required - Less boilerplate - For fasting Development and prototyping, you can use minimal APIs to create endpoints without the need for controllers. 
This approach is suitable for small applications or microservices where simplicity and speed are prioritized.
*/

/*
GET - GetProducts
*/
app.MapGet("/api/products", () =>
{
    var products = new List<Product>
    {
        new Product { Id = 1, Name = "Product 1", Price = 10.99m },
        new Product { Id = 2, Name = "Product 2", Price = 19.99m },
        new Product { Id = 3, Name = "Product 3", Price = 5.99m }
    };

    return Results.Ok(products);
}).WithName("GetProducts");

/*
POST - CreateProduct
*/
app.MapPost("/api/products", (Product product) =>
{
    // In a real application, you would save the product to a database here
    var newProduct = new Product
    {
        Id = new Random().Next(1000, 9999),
        Name = product.Name,
        Price = product.Price
    };
    return Results.Created($"/api/products/{newProduct.Id}", newProduct);
}).WithName("CreateProduct");

/*
PUT - UpdateProduct
*/
app.MapPut("/api/products/{id}", (int id, Product updatedProduct) =>
{
    // In a real application, you would update the product in the database here.
    var product = new Product
    {
        Id = id,
        Name = updatedProduct.Name,
        Price = updatedProduct.Price
    };
    return Results.Ok(product);
}).WithName("UpdateProduct");

/*
Delete - DeleteProduct
*/
app.MapDelete("/api/products/{id}", (int id) =>
{
    // In a real application, you would delete the product from the database here.
    return Results.NoContent();
}).WithName("DeleteProduct");

// END Minimum API Endpoints


app.Run();
