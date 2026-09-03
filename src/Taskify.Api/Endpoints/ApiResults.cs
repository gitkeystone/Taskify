using Taskify.Api.Contracts;

namespace Taskify.Api.Endpoints;

/// <summary>Shared minimal-API response helpers.</summary>
public static class ApiResults
{
    public static IResult NotFound(string message = "Resource not found.") =>
        Results.Json(new ErrorEnvelope(new ApiError("NotFound", message)), statusCode: StatusCodes.Status404NotFound);
}
