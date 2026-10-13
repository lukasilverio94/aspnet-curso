using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class TasksController : ControllerBase
{
    private static readonly List<ProjectTask> Tasks = new List<ProjectTask>
    {
        new ProjectTask { Id = 1, Title = "Implement user authentication", Status = "InProgress", Priority = "High", AssignedTo = "Alice Johnson", CreatedDate = DateTime.Now.AddDays(-5), DueDate = DateTime.Now.AddDays(2) },
        new ProjectTask { Id = 2, Title = "Design dashboard UI", Status = "Done", Priority = "Medium", AssignedTo = "Bob Smith", CreatedDate = DateTime.Now.AddDays(-10), DueDate = DateTime.Now.AddDays(-2) },
        new ProjectTask { Id = 3, Title = "Write API documentation", Status = "Todo", Priority = "Low", AssignedTo = "Charlie Brown", CreatedDate = DateTime.Now.AddDays(-3), DueDate = DateTime.Now.AddDays(7) }
    };

    [HttpGet]
    public IEnumerable<ProjectTask> Get() => Tasks;

    [HttpGet("status/{status}")]
    public IEnumerable<ProjectTask> GetByStatus(string status) =>
        Tasks.Where(t => t.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
}