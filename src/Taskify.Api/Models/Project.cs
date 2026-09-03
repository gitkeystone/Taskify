namespace Taskify.Api.Models;

/// <summary>A container for tasks.</summary>
public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public User? CreatedBy { get; set; }
    public List<TaskItem> Tasks { get; set; } = [];
}
