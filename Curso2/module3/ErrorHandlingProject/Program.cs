using Serilog;

var builder = WebApplication.CreateBuilder(args);

/*
Packages installed on this App: 

dotnet add package Serilog.AspNetCore     
dotnet add package Serilog.Sinks.Console     
dotnet add package Serilog.Sinks.File     
*/
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddControllers();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Global exception caught: {ex.Message}");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");

    }
});

// Enable routing and map controller endpoints
app.UseRouting();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

