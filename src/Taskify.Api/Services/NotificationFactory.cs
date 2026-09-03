using Taskify.Api.Models;

namespace Taskify.Api.Services;

/// <summary>Creates notification records for key events.</summary>
public static class NotificationFactory
{
    public static Notification Create(Guid userId, NotificationType type, string message, Guid? taskId = null, Guid? projectId = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = type,
            Message = message,
            TaskId = taskId,
            ProjectId = projectId,
            IsRead = false,
            CreatedAt = DateTimeOffset.UtcNow
        };
}
