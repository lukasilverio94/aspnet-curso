using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MyApp;

// CUSTOM MIDDLEWARE IN A CLASS 
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var method = context.Request.Method;
        var path = context.Request.Path + context.Request.QueryString;

        var sw = Stopwatch.StartNew();

        try
        {
            _logger.LogDebug("Starting HTTP {Method} {Path}", method, path);

            await _next(context);

            sw.Stop();
            var statusCode = context.Response?.StatusCode;
            var elapsedMs = sw.ElapsedMilliseconds;

            _logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {EllapsedMilliseconds}ms",
                method, path, statusCode, elapsedMs);
        }
        catch (Exception ex)
        {
            sw.Stop();
            var elapsedMs = sw.ElapsedMilliseconds;

            _logger.LogError(
                ex,
                "HTTP {Method} {Path} threw an unhandled exception after {EllapsedMilliseconds}",
                method, path, elapsedMs);

            throw; // rethrow to keep pipeline behavior
        }
    }
}