using Microsoft.EntityFrameworkCore;

namespace TaskManagement.Data;

public class TaskContext : DbContext
{
    public TaskContext(DbContextOptions<TaskContext> options) : base(options) { }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();
}

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}