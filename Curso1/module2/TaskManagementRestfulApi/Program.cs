var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// add Swagger
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
Restful Conventions
TASKS API
*/

// GET /api/tasks - Retrieve a list of tasks
app.MapGet("/api/tasks", () =>
{
    return Results.Ok(new[]
    {
        new { Id = 1, Title = "Task 1", IsComplete = false },
        new { Id = 2, Title = "Task 2", IsComplete = true }
    });
}).WithName("GetTasks");

// GET /api/tasks/{id} - Retrieve a specific task by ID
app.MapGet("/api/tasks/{id}", (int id) =>
{
    var task = new { Id = id, Title = $"Task {id}", IsComplete = false };
    if (task == null)
    {
        return Results.NotFound(new { Error = "Task not found", Message = $"No task with ID {id} exists." });
    }
    return Results.Ok(task);
}).WithName("GetTaskById");

// POST /api/tasks - Create a new task
app.MapPost("/api/tasks", (dynamic task) =>
{
    return Results.Created($"/api/tasks/{task.Id}", task);
}).WithName("CreateTask");

// PUT /api/tasks/{id} - Update an existing task
app.MapPut("/api/tasks/{id}", (int id, dynamic task) =>
{
    return Results.NoContent();
}).WithName("UpdateTask");

// DELETE /api/tasks/{id} - Delete a task
app.MapDelete("/api/tasks/{id}", (int id) =>
{
    return Results.NoContent();
}).WithName("DeleteTask");


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
