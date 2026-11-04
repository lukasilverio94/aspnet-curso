using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;

var builder = WebApplication.CreateBuilder(args);

// Check for connection string from environment variable
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<TaskContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Production-specific optimizations
if (app.Environment.IsProduction())
{
    // Enable response compression
    app.UseResponseCompression();

    // Enable HTTPS redirection
    app.UseHttpsRedirection();

    // Add security headers
    app.Use(async (context, next) =>
   {
       context.Response.Headers["X-Content-Type-Options"] = "nosniff";
       context.Response.Headers["X-Frame-Options"] = "DENY";
       context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
       await next();
   });
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
