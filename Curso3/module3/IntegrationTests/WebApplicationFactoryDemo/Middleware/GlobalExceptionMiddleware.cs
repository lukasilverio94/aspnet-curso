// Middleware/GlobalExceptionMiddleware.cs
using Microsoft.AspNetCore.Mvc;
using WebApplicationFactoryDemo.Models;

namespace WebApplicationFactoryDemo.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ProductNotFoundException ex)
        {
            _logger.LogWarning(ex, "Product not found");
            await WriteProblem(context, StatusCodes.Status404NotFound, "Not Found", ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument");
            await WriteProblem(context, StatusCodes.Status400BadRequest, "Bad Request", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteProblem(context, StatusCodes.Status500InternalServerError, "Internal Server Error",
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblem(HttpContext ctx, int status, string title, string detail)
    {
        if (ctx.Response.HasStarted) return;
        ctx.Response.Clear();
        ctx.Response.StatusCode = status;

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = ctx.Request.Path
        };

        await ctx.Response.WriteAsJsonAsync(
            problem,
            options: null,
            contentType: "application/problem+json");
    }
}