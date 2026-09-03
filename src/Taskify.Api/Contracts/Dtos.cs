using Taskify.Api.Models;

namespace Taskify.Api.Contracts;

// ---- Users ----

public record UserDto(Guid Id, string Name, UserRole Role);

// ---- Projects ----

public record ProjectDto(Guid Id, string Name, Guid CreatedById, DateTimeOffset CreatedAt);

public record CreateProjectRequest(string Name);

// ---- Tasks ----

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

public record CreateTaskRequest(string Title, string? Description);

public record UpdateTaskRequest(
    string? Title,
    string? Description,
    Guid? AssigneeId,
    KanbanColumn? Status,
    bool ClearAssignee);

// ---- Comments ----

public record CommentDto(Guid Id, Guid TaskId, Guid AuthorId, string Text, DateTimeOffset CreatedAt);

public record AddCommentRequest(string Text);

// ---- Notifications ----

public record NotificationDto(
    Guid Id,
    Guid UserId,
    NotificationType Type,
    string Message,
    Guid? TaskId,
    Guid? ProjectId,
    bool IsRead,
    DateTimeOffset CreatedAt);

public record MarkReadRequest(bool IsRead);

// ---- Errors ----

public record ApiError(string Code, string Message, IReadOnlyDictionary<string, string[]>? Fields = null);

public record ErrorEnvelope(ApiError Error);
