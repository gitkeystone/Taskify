using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Taskify.Api.Data;
using Taskify.Api.Endpoints;
using Taskify.Api.Validation;

var builder = WebApplication.CreateBuilder(args);

// Shared Aspire service defaults (health checks, OpenTelemetry, resilience, service discovery).
builder.AddServiceDefaults();

builder.Services.AddOpenApi();

// PostgreSQL system of record. Connection string is injected by the Aspire AppHost as
// "ConnectionStrings__taskify"; fall back to a local connection for direct `dotnet run`.
var connectionString = builder.Configuration.GetConnectionString("taskify")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "No database connection string configured. Run via the Aspire AppHost or set ConnectionStrings__taskify.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

// Server-side input validation (constitution Principle II).
builder.Services.AddValidatorsFromAssemblyContaining<CreateProjectRequestValidator>();

// Serialize enums (KanbanColumn, UserRole, NotificationType) as their string names.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Create the schema and seed the five users + three sample projects on startup (MVP).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await SeedData.SeedAsync(db);
}

app.MapUsersEndpoints();
app.MapProjectsEndpoints();
app.MapTasksEndpoints();
app.MapCommentsEndpoints();
app.MapNotificationsEndpoints();

app.Run();
