var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL system of record (persistent volume so data survives restarts).
var postgres = builder.AddPostgres("taskify-postgres")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var taskifyDb = postgres.AddDatabase("taskify");

// REST API microservice: owns the database and exposes projects/tasks/notifications/users.
var api = builder.AddProject<Projects.Taskify_Api>("apiservice")
    .WithReference(taskifyDb)
    .WaitFor(taskifyDb);

// Blazor Server front end: calls the API over REST (never touches Postgres directly).
builder.AddProject<Projects.Taskify_Web>("web")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
