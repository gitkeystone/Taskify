using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Taskify.Api.Contracts;
using Taskify.Api.Data;
using Taskify.Api.Identity;
using Taskify.Api.Models;
using Taskify.Api.Services;
using Taskify.Api.Validation;

namespace Taskify.Api.Endpoints;

/// <summary>REST endpoints for task comments.</summary>
public static class CommentsEndpoints
{
    public static void MapCommentsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tasks/{taskId:guid}/comments").WithTags("Comments");

        group.MapGet("/", async (Guid taskId, ApplicationDbContext db) =>
        {
            if (!await db.Tasks.AnyAsync(t => t.Id == taskId))
            {
                return ApiResults.NotFound("Task not found.");
            }

            var comments = await db.Comments.Where(c => c.TaskId == taskId).OrderBy(c => c.CreatedAt).ToListAsync();
            return Results.Ok(comments.Select(c => c.ToDto()));
        });

        group.MapPost("/", async (Guid taskId, AddCommentRequest request, IValidator<AddCommentRequest> validator, ApplicationDbContext db, HttpContext http) =>
        {
            var task = await db.Tasks.FindAsync(taskId);
            if (task is null)
            {
                return ApiResults.NotFound("Task not found.");
            }

            var normalized = new AddCommentRequest((request.Text ?? string.Empty).Trim());
            var validation = await validator.ValidateAsync(normalized);
            if (!validation.IsValid)
            {
                return validation.ToValidationError();
            }

            var currentUserId = CurrentUser.Id(http);

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                AuthorId = currentUserId,
                Text = normalized.Text,
                CreatedAt = DateTimeOffset.UtcNow
            };

            db.Comments.Add(comment);

            // Notify the task's assignee and creator (excluding the commenter) about the new comment.
            var commenter = await db.Users.FindAsync(currentUserId);
            var commenterName = commenter?.Name ?? "A team member";

            var recipients = new[] { task.AssigneeId, task.CreatedById }
                .Where(id => id is not null && id != currentUserId)
                .Select(id => id!.Value)
                .Distinct();

            foreach (var recipient in recipients)
            {
                db.Notifications.Add(NotificationFactory.Create(recipient, NotificationType.TaskCommented, $"{commenterName} commented on \"{task.Title}\".", task.Id, task.ProjectId));
            }

            await db.SaveChangesAsync();

            return Results.Created($"/api/tasks/{taskId}/comments/{comment.Id}", comment.ToDto());
        }).AddEndpointFilter<RequireUserFilter>();
    }
}
