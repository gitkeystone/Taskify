using FluentValidation.Results;
using Taskify.Api.Contracts;

namespace Taskify.Api.Validation;

/// <summary>Converts FluentValidation results into the standard error envelope.</summary>
public static class ValidationResultExtensions
{
    public static IResult ToValidationError(this ValidationResult result)
    {
        var fields = result.Errors
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());

        return Results.Json(
            new ErrorEnvelope(new ApiError("ValidationFailed", "One or more fields are invalid.", fields)),
            statusCode: StatusCodes.Status400BadRequest);
    }
}
