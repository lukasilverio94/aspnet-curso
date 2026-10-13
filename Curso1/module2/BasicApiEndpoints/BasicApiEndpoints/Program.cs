var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Root path!");
app.MapGet("/downloads", () => "Downloads path!");
app.MapPut("/", () => "Root path with PUT method!");
app.MapDelete("/", () => "Root path with DELETE method!");
app.MapPost("/", () => "Root path with POST method!");

app.Run();
