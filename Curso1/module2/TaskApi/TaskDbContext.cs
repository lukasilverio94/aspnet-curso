using Microsoft.EntityFrameworkCore;

namespace TaskApi;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
    {

    }

    // Parameterless constructor for EF design-time tools
    public TaskDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=tasks.db");
        }
    }

    public DbSet<Task> Tasks { get; set; }

}