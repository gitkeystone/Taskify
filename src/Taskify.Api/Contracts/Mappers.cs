using Taskify.Api.Models;

namespace Taskify.Api.Contracts;

/// <summary>Maps domain entities to API response DTOs.</summary>
public static class Mappers
{
    public static UserDto ToDto(this User u) => new(u.Id, u.Name, u.Role);

    public static ProjectDto ToDto(this Project p) => new(p.Id, p.Name, p.CreatedById, p.CreatedAt);

    public static TaskDto ToDto(this TaskItem t) =>
        new(t.Id, t.ProjectId, t.Title, t.Description, t.Status, t.AssigneeId, t.CreatedById, t.CreatedAt, t.UpdatedAt);

    public static CommentDto ToDto(this Comment c) => new(c.Id, c.TaskId, c.AuthorId, c.Text, c.CreatedAt);

    public static NotificationDto ToDto(this Notification n) =>
        new(n.Id, n.UserId, n.Type, n.Message, n.TaskId, n.ProjectId, n.IsRead, n.CreatedAt);
}
