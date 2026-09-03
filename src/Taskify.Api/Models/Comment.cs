namespace Taskify.Api.Models;

/// <summary>A text note attached to a task.</summary>
public class Comment
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid AuthorId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }

    public TaskItem? Task { get; set; }
    public User? Author { get; set; }
}
