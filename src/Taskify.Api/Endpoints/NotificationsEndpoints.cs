using Microsoft.EntityFrameworkCore;
using Taskify.Api.Contracts;
using Taskify.Api.Data;
using Taskify.Api.Identity;

namespace Taskify.Api.Endpoints;

/// <summary>REST endpoints for in-app notifications.</summary>
public static class NotificationsEndpoints
{
    public static void MapNotificationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/notifications").WithTags("Notifications");

        group.MapGet("/", async (Guid userId, bool? unreadOnly, ApplicationDbContext db) =>
        {
            var query = db.Notifications.Where(n => n.UserId == userId);
            if (unreadOnly == true)
            {
                query = query.Where(n => !n.IsRead);
            }

            var notifications = await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
            return Results.Ok(notifications.Select(n => n.ToDto()));
        });

        group.MapPatch("/{id:guid}/read", async (Guid id, MarkReadRequest request, ApplicationDbContext db) =>
        {
            var notification = await db.Notifications.FindAsync(id);
            if (notification is null)
            {
                return ApiResults.NotFound("Notification not found.");
            }

            notification.IsRead = request.IsRead;
            await db.SaveChangesAsync();

            return Results.Ok(notification.ToDto());
        }).AddEndpointFilter<RequireUserFilter>();
    }
}
