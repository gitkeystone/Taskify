namespace Taskify.Api.Models;

/// <summary>
/// The four Kanban columns a task can occupy. Ordered To Do → In Progress → In Review → Done.
/// </summary>
public enum KanbanColumn
{
    ToDo = 1,
    InProgress = 2,
    InReview = 3,
    Done = 4
}
