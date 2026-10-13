public class ProjectTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // "Todo", "InProgress", "Done"
    public string Priority { get; set; } = string.Empty; // "Low", "Medium", "High"
    public string AssignedTo { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime? DueDate { get; set; }
}