using TaskAPI.Models;
using TaskAPI.Services;
using TaskAPI.Middlewares;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// Add Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ITaskService, InMemoryTaskService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// register middleware
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

/*
******* Tasks Endpoint ******* 
******* MINIMUM API PATTERN *******
*/
app.MapGet("/api/tasks", async (ITaskService taskService) =>
{
    var tasks = await taskService.GetAllTasksAsync();
    return Results.Ok(tasks);
})
.WithName("GetAllTasks");

app.MapGet("/api/tasks/{id:int}", async (int id, ITaskService taskService) =>
{
    var task = await taskService.GetTaskByIdAsync(id);
    return task is not null ? Results.Ok(task) : Results.NotFound($"Task {id} not found");
})
.WithName("GetTaskById");

app.MapGet("/api/tasks/category/{categoryId:int}", async (int categoryId, ITaskService taskService) =>
{
    var tasks = await taskService.GetTasksByCategoryAsync(categoryId);
    return Results.Ok(tasks);
})
.WithName("GetTasksByCategory");

app.MapPost("/api/tasks", async (TaskItem task, ITaskService taskService) =>
{
    // Validate the model
    var validationResults = new List<ValidationResult>();
    var context = new ValidationContext(task);

    if (!Validator.TryValidateObject(task, context, validationResults, true))
    {
        return Results.BadRequest(validationResults.Select(v => v.ErrorMessage));
    }

    try
    {
        var createdTask = await taskService.CreateTaskAsync(task);
        return Results.Created($"/api/tasks/{createdTask.Id}", createdTask);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
})
.WithName("CreateTask");

app.MapPut("/api/tasks/{id:int}", async (int id, TaskItem task, ITaskService taskService) =>
{
    // Validate the model
    var validationResults = new List<ValidationResult>();
    var context = new ValidationContext(task);

    if (!Validator.TryValidateObject(task, context, validationResults, true))
    {
        return Results.BadRequest(validationResults.Select(v => v.ErrorMessage));
    }

    try
    {
        var updatedTask = await taskService.UpdateTaskAsync(id, task);
        return updatedTask is not null ? Results.Ok(updatedTask) : Results.NotFound($"Task {id} not found");
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
})
.WithName("UpdateTask");

app.MapDelete("/api/tasks/{id:int}", async (int id, ITaskService taskService) =>
{
    var deleted = await taskService.DeleteTaskAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound($"Task {id} not found");
})
.WithName("DeleteTask");

// Category endpoints
app.MapGet("/api/categories", async (ITaskService taskService) =>
{
    var categories = await taskService.GetAllCategoriesAsync();
    return Results.Ok(categories);
})
.WithName("GetAllCategories");

app.MapGet("/api/categories/{id:int}", async (int id, ITaskService taskService) =>
{
    var category = await taskService.GetCategoryByIdAsync(id);
    return category is not null ? Results.Ok(category) : Results.NotFound($"Category {id} not found");
})
.WithName("GetCategoryById");

app.MapPost("/api/categories", async (Category category, ITaskService taskService) =>
{
    // Validate the model
    var validationResults = new List<ValidationResult>();
    var context = new ValidationContext(category);

    if (!Validator.TryValidateObject(category, context, validationResults, true))
    {
        return Results.BadRequest(validationResults.Select(v => v.ErrorMessage));
    }

    var createdCategory = await taskService.CreateCategoryAsync(category);
    return Results.Created($"/api/categories/{createdCategory.Id}", createdCategory);
})
.WithName("CreateCategory");

app.Run();