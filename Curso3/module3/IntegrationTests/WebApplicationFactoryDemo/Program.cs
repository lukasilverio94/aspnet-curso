using WebApplicationFactoryDemo.Middleware;
using WebApplicationFactoryDemo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddScoped<IProductService, ProductService>();

// Optional: outbound HttpClient with a circuit breaker — this is what the
// circuit-breaker tests will hit (an "external" dependency).
builder.Services.AddHttpClient("external", c =>
{
    c.BaseAddress = new Uri("https://example.com/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ORDER MATTERS: exception middleware must be early (upstream) so it wraps
// routing, controllers, and services (all downstream).
app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

app.Run();