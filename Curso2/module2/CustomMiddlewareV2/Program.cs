var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseRouting();

// Register Middleware direcetly
app.UseMiddleware<MyApp.RequestLoggingMiddleware>();

app.MapControllers();

app.MapGet("/", () => "I am root!");
app.MapGet("/dashboard", () => "I am the dashboard");

app.Run();

