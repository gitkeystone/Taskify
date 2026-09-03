namespace Taskify.Web.Services;

/// <summary>The four Kanban columns, matching the API enum names.</summary>
public enum KanbanColumn { ToDo, InProgress, InReview, Done }

public enum UserRole { ProductManager, Engineer }

public enum NotificationType { TaskAssigned, TaskStatusChanged, TaskCommented }

public static class KanbanColumns
{
    public static readonly KanbanColumn[] All = [KanbanColumn.ToDo, KanbanColumn.InProgress, KanbanColumn.InReview, KanbanColumn.Done];

    public static string DisplayName(this KanbanColumn column) => column switch
    {
        KanbanColumn.ToDo => "To Do",
        KanbanColumn.InProgress => "In Progress",
        KanbanColumn.InReview => "In Review",
        KanbanColumn.Done => "Done",
        _ => column.ToString()
    };
}

public record UserDto(Guid Id, string Name, UserRole Role);

public record ProjectDto(Guid Id, string Name, Guid CreatedById, DateTimeOffset CreatedAt);

public record TaskDto(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    KanbanColumn Status,
    Guid? AssigneeId,
    Guid CreatedById,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public record CommentDto(Guid Id, Guid TaskId, Guid AuthorId, string Text, DateTimeOffset CreatedAt);

public record NotificationDto(
    Guid Id,
    Guid UserId,
    NotificationType Type,
    string Message,
    Guid? TaskId,
    Guid? ProjectId,
    bool IsRead,
    DateTimeOffset CreatedAt);

public record CreateProjectRequest(string Name);

public record CreateTaskRequest(string Title, string? Description);

public record UpdateTaskRequest(string? Title, string? Description, Guid? AssigneeId, KanbanColumn? Status, bool ClearAssignee);

public record AddCommentRequest(string Text);

public record MarkReadRequest(bool IsRead);
