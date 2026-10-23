using TaskAPI.Models;
using System.Collections.Concurrent;
namespace TaskAPI.Services;

public class InMemoryTaskService : ITaskService
{
    private readonly ConcurrentDictionary<int, TaskItem> _tasks = new();
    private readonly ConcurrentDictionary<int, Category> _categories = new();
    private int _nextTaskId = 1;
    private int _nextCategoryId = 1;

    public InMemoryTaskService()
    {
        SeedData();
    }

    private void SeedData()
    {
        var categories = new[]
        {
            new Category { Id = 1, Name = "Personal", Description = "Personal tasks" },
            new Category { Id = 2, Name = "Work", Description = "Work-related tasks" },
            new Category { Id = 3, Name = "Shopping", Description = "Shopping lists" }
        };

        foreach (var category in categories)
        {
            _categories.TryAdd(category.Id, category);
            _nextCategoryId = Math.Max(_nextCategoryId, category.Id + 1);
        }
    }

    // Task methods
    public Task<IEnumerable<TaskItem>> GetAllTasksAsync()
    {
        return Task.FromResult(_tasks.Values.AsEnumerable());
    }

    public Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        _tasks.TryGetValue(id, out var task);
        return Task.FromResult(task);
    }
    public Task<IEnumerable<TaskItem>> GetTasksByCategoryAsync(int categoryId)
    {
        var tasks = _tasks.Values.Where(t => t.CategoryId == categoryId);
        return Task.FromResult(tasks);
    }
    public Task<TaskItem> CreateTaskAsync(TaskItem task)
    {
        // Validate category exists
        if (!_categories.ContainsKey(task.CategoryId))
        {
            throw new ArgumentException($"Category {task.CategoryId} does not exist");
        }
        task.Id = _nextTaskId++;
        task.CreatedAt = DateTime.UtcNow;
        _tasks.TryAdd(task.Id, task);
        return Task.FromResult(task);
    }
    public Task<TaskItem?> UpdateTaskAsync(int id, TaskItem task)
    {
        if (!_tasks.ContainsKey(id))
        {
            return Task.FromResult<TaskItem?>(null);
        }
        // Validate category exists
        if (!_categories.ContainsKey(task.CategoryId))
        {
            throw new ArgumentException($"Category {task.CategoryId} does not exist");
        }
        task.Id = id;
        _tasks.TryUpdate(id, task, _tasks[id]);
        return Task.FromResult<TaskItem?>(task);
    }
    public Task<bool> DeleteTaskAsync(int id)
    {
        var removed = _tasks.TryRemove(id, out _);
        return Task.FromResult(removed);
    }


    // Category methods
    public Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return Task.FromResult(_categories.Values.AsEnumerable());
    }
    public Task<Category?> GetCategoryByIdAsync(int id)
    {
        _categories.TryGetValue(id, out var category);
        return Task.FromResult(category);
    }
    public Task<Category> CreateCategoryAsync(Category category)
    {
        category.Id = _nextCategoryId++;
        _categories.TryAdd(category.Id, category);
        return Task.FromResult(category);
    }

}