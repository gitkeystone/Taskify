# Taskify

A team productivity platform where predefined users manage work on a Kanban board — create
projects and tasks, move tasks across **To Do → In Progress → In Review → Done**, assign tasks
to teammates, and comment on them. This first phase ships with five predefined users (one
product manager, four engineers), three sample projects, and **no login** (you pick who you are
acting as).

Built on **.NET 10 LTS / .NET Aspire 13** with a **Blazor Server** front end and **PostgreSQL**.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (any 10.x)
- A running **PostgreSQL** instance — Taskify connects to it; it does not provision one.

> **Note**: if the SDK was installed with `dotnet-install.sh`, it lives under `~/.dotnet` and
> may not be on your `PATH`. Add it first:
>
> ```bash
> export PATH="$HOME/.dotnet:$PATH"
> ```

> **Configure your database**: point the app at your Postgres by editing
> `src/Taskify.AppHost/appsettings.json` → `ConnectionStrings:taskify`, e.g.:
>
> ```
> Host=localhost;Port=5432;Database=taskify;Username=postgres;Password=your_password
> ```
>
> On first startup the app runs `MigrateAsync()` and seeds five users + three sample projects.
> Use a `ConnectionStrings__taskify` environment variable (or user-secrets) to override the
> password outside local development.

## Start the app

From the repository root, launch the Aspire AppHost. It starts the REST API and the Blazor
web app, which connect to your configured PostgreSQL:

```bash
dotnet run --project src/Taskify.AppHost
```

What happens next:

1. The **Aspire dashboard** opens in your browser, listing the running resources
   (`apiservice`, `web`).
2. Open the **`web`** resource from the dashboard (its URL is shown there) — that is the
   Taskify board.
3. The app seeds itself on first run: five users and three sample projects.

> The API's OpenAPI (Swagger) UI is available at the `apiservice` URL under `/swagger`.

## Using the app

1. On the **Projects** page, choose **"You are acting as"** to select one of the five users
   (this is the no-login identity).
2. Pick a sample project (or create a new one) to open its **Kanban board**.
3. Add tasks (they start in **To Do**), drag cards between columns (or use each card's
   **Status** dropdown), assign an **Assignee**, and add **Comments**.
4. The **Notifications** page shows events like "you were assigned …" and task moves; changes
   made in one browser appear in others in near-real time.

## Tests

```bash
# API validation unit tests
dotnet test tests/Taskify.Api.Tests

# Blazor component tests (bUnit)
dotnet test tests/Taskify.Web.Tests

# End-to-end smoke tests (boots the full Aspire stack; requires Docker)
dotnet test tests/Taskify.E2E.Tests
```

## Architecture

| Project | Role |
|---------|------|
| `src/Taskify.AppHost/` | Aspire orchestrator (services + PostgreSQL) |
| `src/Taskify.ServiceDefaults/` | Shared health checks, OpenTelemetry, resilience, service discovery |
| `src/Taskify.Api/` | REST API (system of record; the only service with DB access) |
| `src/Taskify.Web/` | Blazor Server UI (drag-and-drop board, real-time updates) |
| `tests/` | xUnit (validators), bUnit (components), Aspire (E2E) |

The Blazor app never touches the database directly — it calls the API over REST. Server-side
input validation (FluentValidation) and a no-login identity guard (`X-Taskify-User-Id`) are
enforced at the API boundary.

## Troubleshooting

If the Aspire Dashboard shows **"The remote certificate is invalid … UntrustedRoot"** (or the
app logs *"ASP.NET Core developer certificate is not trusted"*), the local HTTPS development
certificate isn't trusted yet. Trust it once:

```bash
dotnet dev-certs https --trust
```

On Linux you may also need to add it to the system CA store:

```bash
sudo cp ~/.aspnet/dev-certs/trust/*.pem /usr/local/share/ca-certificates/aspnetcore-localhost.crt
sudo update-ca-certificates
```

If it still fails, set the per-user OpenSSL trust path before running:

```bash
export SSL_CERT_DIR="$HOME/.aspnet/dev-certs/trust:/usr/lib/ssl/certs"
```

Then re-run `dotnet run --project src/Taskify.AppHost`.

## Project documentation

Spec-Kit artifacts live under [`specs/001-kanban-task-management/`](specs/001-kanban-task-management/):

- `spec.md` — feature specification
- `plan.md` — implementation plan & architecture
- `tasks.md` — implementation tasks
- `contracts/api-contracts.md` — REST API contract
- `data-model.md` — entities & relationships
- `quickstart.md` — end-to-end validation scenarios

Security notes (including the phase-1 no-login deviation and phase-2 plan) are in
[`SECURITY.md`](SECURITY.md).
