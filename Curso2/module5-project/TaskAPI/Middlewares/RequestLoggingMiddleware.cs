namespace TaskAPI.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;

            _logger.LogInformation("Request: {Method} {Path} start at {Time}",
                context.Request.Method,
                context.Request.Path,
                startTime);

            await _next(context);

            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("Request: {Method} {Path} completed in {Duration}ms with status {StatusCode}",
                context.Request.Method,
                context.Request.Path,
                Math.Abs(duration.TotalMilliseconds),
                context.Response.StatusCode);
        }
    }
}