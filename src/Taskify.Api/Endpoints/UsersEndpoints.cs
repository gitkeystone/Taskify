using Microsoft.EntityFrameworkCore;
using Taskify.Api.Contracts;
using Taskify.Api.Data;

namespace Taskify.Api.Endpoints;

/// <summary>Read-only endpoints for the predefined users.</summary>
public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapGet("/", async (ApplicationDbContext db) =>
            (await db.Users.OrderBy(u => u.Name).ToListAsync()).Select(u => u.ToDto()));
    }
}
