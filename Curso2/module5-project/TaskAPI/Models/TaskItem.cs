using System.ComponentModel.DataAnnotations;
namespace TaskAPI.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    [Required]
    public TaskStatus Status { get; set; } = TaskStatus.Pending;

    [Required]
    public int CategoryId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public enum TaskStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2
}

