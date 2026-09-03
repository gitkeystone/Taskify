using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Taskify.Api.Contracts;
using Taskify.Api.Data;
using Taskify.Api.Identity;
using Taskify.Api.Models;
using Taskify.Api.Services;
using Taskify.Api.Validation;

namespace Taskify.Api.Endpoints;

/// <summary>REST endpoints for tasks.</summary>
public static class TasksEndpoints
{
    public static void MapTasksEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api").WithTags("Tasks");

        group.MapGet("/projects/{projectId:guid}/tasks", async (Guid projectId, KanbanColumn? status, ApplicationDbContext db) =>
        {
            if (!await db.Projects.AnyAsync(p => p.Id == projectId))
            {
                return ApiResults.NotFound("Project not found.");
            }

            var query = db.Tasks.Where(t => t.ProjectId == projectId);
            if (status is not null)
            {
                query = query.Where(t => t.Status == status);
            }

            var tasks = await query.OrderBy(t => t.CreatedAt).ToListAsync();
            return Results.Ok(tasks.Select(t => t.ToDto()));
        });

        group.MapPost("/projects/{projectId:guid}/tasks", async (Guid projectId, CreateTaskRequest request, IValidator<CreateTaskRequest> validator, ApplicationDbContext db, HttpContext http) =>
        {
            if (!await db.Projects.AnyAsync(p => p.Id == projectId))
            {
                return ApiResults.NotFound("Project not found.");
            }

            var normalized = new CreateTaskRequest((request.Title ?? string.Empty).Trim(), request.Description?.Trim());
            var validation = await validator.ValidateAsync(normalized);
            if (!validation.IsValid)
            {
                return validation.ToValidationError();
            }

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                Title = normalized.Title,
                Description = normalized.Description,
                Status = KanbanColumn.ToDo,
                CreatedById = CurrentUser.Id(http),
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            db.Tasks.Add(task);
            await db.SaveChangesAsync();

            return Results.Created($"/api/tasks/{task.Id}", task.ToDto());
        }).AddEndpointFilter<RequireUserFilter>();

        group.MapGet("/tasks/{id:guid}", async (Guid id, ApplicationDbContext db) =>
        {
            var task = await db.Tasks.FindAsync(id);
            return task is null ? ApiResults.NotFound("Task not found.") : Results.Ok(task.ToDto());
        });

        group.MapPatch("/tasks/{id:guid}", async (Guid id, UpdateTaskRequest request, IValidator<UpdateTaskRequest> validator, ApplicationDbContext db, HttpContext http) =>
        {
            var task = await db.Tasks.FindAsync(id);
            if (task is null)
            {
                return ApiResults.NotFound("Task not found.");
            }

            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return validation.ToValidationError();
            }

            var currentUserId = CurrentUser.Id(http);
            var previousStatus = task.Status;
            var previousAssignee = task.AssigneeId;

            if (request.Title is not null)
            {
                task.Title = request.Title.Trim();
            }

            if (request.Description is not null)
            {
                task.Description = request.Description.Trim();
            }

            if (request.ClearAssignee)
            {
                task.AssigneeId = null;
            }
            else if (request.AssigneeId is { } newAssignee)
            {
                if (!await db.Users.AnyAsync(u => u.Id == newAssignee))
                {
                    return Results.Json(
                        new ErrorEnvelope(new ApiError("ValidationFailed", "Assignee is not a known user.", new Dictionary<string, string[]> { ["assigneeId"] = ["Assignee is not a known user."] })),
                        statusCode: StatusCodes.Status400BadRequest);
                }

                task.AssigneeId = newAssignee;
            }

            if (request.Status is { } newStatus)
            {
                task.Status = newStatus;
            }

            task.UpdatedAt = DateTimeOffset.UtcNow;

            // Generate notifications for assignee/status changes.
            if (request.Status is { } changedStatus && changedStatus != previousStatus && task.AssigneeId is { } assigneeId && assigneeId != currentUserId)
            {
                db.Notifications.Add(NotificationFactory.Create(assigneeId, NotificationType.TaskStatusChanged, $"Task \"{task.Title}\" moved to {changedStatus}.", task.Id, task.ProjectId));
            }

            if (task.AssigneeId is { } assignedId && assignedId != previousAssignee && assignedId != currentUserId)
            {
                db.Notifications.Add(NotificationFactory.Create(assignedId, NotificationType.TaskAssigned, $"You were assigned \"{task.Title}\".", task.Id, task.ProjectId));
            }

            await db.SaveChangesAsync();

            return Results.Ok(task.ToDto());
        }).AddEndpointFilter<RequireUserFilter>();
    }
}
