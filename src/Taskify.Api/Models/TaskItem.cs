namespace Taskify.Api.Models;

/// <summary>
/// A unit of work. Belongs to exactly one project and occupies exactly one Kanban column.
/// Named <c>TaskItem</c> to avoid collision with <see cref="System.Threading.Tasks.Task"/>.
/// </summary>
public class TaskItem
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public KanbanColumn Status { get; set; } = KanbanColumn.ToDo;
    public Guid? AssigneeId { get; set; }
    public Guid CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Project? Project { get; set; }
    public User? Assignee { get; set; }
    public User? CreatedBy { get; set; }
    public List<Comment> Comments { get; set; } = [];
}
