var builder = DistributedApplication.CreateBuilder(args);

// Connect to an existing PostgreSQL service via the connection string in
// src/Taskify.AppHost/appsettings.json -> ConnectionStrings:taskify.
// Edit that value to point at your Postgres (host/port/database/user/password).
var postgres = builder.AddConnectionString("taskify");

// REST API microservice: owns the database and exposes projects/tasks/notifications/users.
var api = builder.AddProject<Projects.Taskify_Api>("apiservice")
    .WithReference(postgres);

// Blazor Server front end: calls the API over REST (never touches Postgres directly).
builder.AddProject<Projects.Taskify_Web>("web")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
