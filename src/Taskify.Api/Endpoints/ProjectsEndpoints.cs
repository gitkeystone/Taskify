using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Taskify.Api.Contracts;
using Taskify.Api.Data;
using Taskify.Api.Identity;
using Taskify.Api.Models;
using Taskify.Api.Validation;

namespace Taskify.Api.Endpoints;

/// <summary>REST endpoints for projects.</summary>
public static class ProjectsEndpoints
{
    public static void MapProjectsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects").WithTags("Projects");

        group.MapGet("/", async (ApplicationDbContext db) =>
            (await db.Projects.OrderBy(p => p.CreatedAt).ToListAsync()).Select(p => p.ToDto()));

        group.MapGet("/{id:guid}", async (Guid id, ApplicationDbContext db) =>
        {
            var project = await db.Projects.FindAsync(id);
            return project is null ? ApiResults.NotFound("Project not found.") : Results.Ok(project.ToDto());
        });

        group.MapPost("/", async (CreateProjectRequest request, IValidator<CreateProjectRequest> validator, ApplicationDbContext db, HttpContext http) =>
        {
            var normalized = new CreateProjectRequest((request.Name ?? string.Empty).Trim());
            var validation = await validator.ValidateAsync(normalized);
            if (!validation.IsValid)
            {
                return validation.ToValidationError();
            }

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = normalized.Name,
                CreatedById = CurrentUser.Id(http),
                CreatedAt = DateTimeOffset.UtcNow
            };

            db.Projects.Add(project);
            await db.SaveChangesAsync();

            return Results.Created($"/api/projects/{project.Id}", project.ToDto());
        }).AddEndpointFilter<RequireUserFilter>();
    }
}
