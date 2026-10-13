using Microsoft.EntityFrameworkCore;
using TaskApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register TaskDbContext with SQLite connection string with Dependency Injection (DI) container
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlite("Data Source=tasks.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


/*
 * Task API Endpoints ****

 GET AlL Tasks: GET /api/task
 */
app.MapGet("/api/tasks", async (TaskDbContext db) =>
{
    return await db.Tasks.ToListAsync();

})
.WithName("GetTasks");

// GET Task by ID: GET /api/tasks/{id}
app.MapGet("/api/tasks/{id}", async (int id, TaskDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    return task is not null ? Results.Ok(task) : Results.NotFound();
})
.WithName("GetTaskById");

// POST Create a new Task: POST /api/tasks
app.MapPost("/api/tasks", async (TaskApi.Task task, TaskDbContext db) =>
{
    db.Tasks.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tasks/{task.Id}", task);
})
.WithName("CreateTask");

// PUT Update an existing Task: PUT /api/tasks/{id}
app.MapPut("/api/tasks/{id}", async (int id, TaskApi.Task updatedTask, TaskDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task is null) return Results.NotFound();

    task.Title = updatedTask.Title;
    task.Description = updatedTask.Description;
    task.IsCompleted = updatedTask.IsCompleted;

    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithDescription("Update Task");

// DELETE Delete a Task: DELETE /api/tasks/{id}
app.MapDelete("/api/tasks/{id}", async (int id, TaskDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task is null) return Results.NotFound();

    db.Tasks.Remove(task);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithDescription("Delete Task");

// End Tasks Endpoints ****

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
