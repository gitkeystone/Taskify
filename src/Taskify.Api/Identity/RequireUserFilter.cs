using Microsoft.AspNetCore.Http;
using Taskify.Api.Contracts;
using Taskify.Api.Data;

namespace Taskify.Api.Identity;

/// <summary>
/// Validates the <c>X-Taskify-User-Id</c> header against the seeded users and stores the
/// current user id for the request. This is the phase-1 (no-login) identity guard.
/// </summary>
public sealed class RequireUserFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;

        if (!httpContext.Request.Headers.TryGetValue("X-Taskify-User-Id", out var raw) ||
            !Guid.TryParse(raw.ToString(), out var userId))
        {
            return Unauthorized("A valid X-Taskify-User-Id header is required.");
        }

        var db = httpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
        var user = await db.Users.FindAsync(userId);
        if (user is null)
        {
            return Unauthorized("Unknown user.");
        }

        httpContext.Items[CurrentUser.ItemKey] = userId;
        return await next(context);
    }

    private static IResult Unauthorized(string message) =>
        Results.Json(new ErrorEnvelope(new ApiError("Unauthorized", message)), statusCode: StatusCodes.Status401Unauthorized);
}

/// <summary>Helpers for reading the current user from <see cref="HttpContext.Items"/>.</summary>
public static class CurrentUser
{
    public const string ItemKey = "CurrentUserId";

    public static Guid Id(HttpContext httpContext)
    {
        if (httpContext.Items.TryGetValue(ItemKey, out var value) && value is Guid id)
        {
            return id;
        }

        throw new InvalidOperationException("Current user has not been established for this request.");
    }
}
