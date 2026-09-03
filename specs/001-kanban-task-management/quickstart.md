# Quickstart & Validation Guide: Kanban Task Management

Phase 1 output. Runnable, end-to-end validation scenarios that prove the feature works. See
[data-model.md](./data-model.md) and [contracts/api-contracts.md](./contracts/api-contracts.md)
for details; implementation specifics live in `tasks.md` and the implementation phase.

## Prerequisites

- .NET 10 SDK (LTS)
- Docker (or Podman) with a running daemon — used by Aspire to provision PostgreSQL
- A terminal at the repository root

## Run the app

```bash
# Build the solution
dotnet build Taskify.sln

# Launch via the Aspire AppHost (starts Taskify.Api, Taskify.Web, and Postgres)
dotnet run --project src/Taskify.AppHost
```

The Aspire dashboard opens and lists the services. Open `Taskify.Web` (Blazor) in a browser —
this is the Taskify board UI. OpenAPI for `Taskify.Api` is available at its `/swagger` endpoint.

## Validation scenarios

Each scenario maps to the spec's success criteria and acceptance tests.

### 1. Identity selection + sample data (SC-002, US5)

1. Open the app with no login.
2. **Expect**: you are offered exactly five identities (one Product Manager, four Engineers) and
   the three sample projects are visible immediately.

### 2. Create a project and a task (SC-001, US1)

1. Select any identity.
2. Create a project named "Sprint 1".
3. Open it and add a task titled "Set up CI".
4. **Expect**: the task appears in the **To Do** column.

### 3. Move tasks across columns (US2, FR-008)

1. Drag the task card from **To Do** to **In Progress**, then to **Done**, then back to
   **In Progress**.
2. **Expect**: the task always appears in exactly one column; the four columns remain visible;
   a second browser (different identity) reflects the move in near-real-time (< 500 ms).

### 4. Assign and comment (US3, US4, FR-009, FR-010)

1. Assign the task to an engineer; reassign to a different user; clear the assignment.
2. Add a comment; open the task and confirm the comment shows author + time in chronological
   order.
3. **Expect**: empty comments and invalid titles are rejected with a clear message and nothing
   is saved.

### 5. Notifications (plan scope addition)

1. As User A, assign a task to User B.
2. Open User B's notifications.
3. **Expect**: a "You were assigned …" notification appears; marking it read persists.

### 6. Persistence (SC-004)

1. Refresh the page (and/or restart `dotnet run`).
2. **Expect**: projects, tasks, assignments, comments, and column positions are intact.

## Automated tests

```bash
# API unit + integration tests (validation + endpoint behavior)
dotnet test tests/Taskify.Api.Tests

# Blazor component tests (board render, drag/drop, empty states)
dotnet test tests/Taskify.Web.Tests

# End-to-end smoke test via Aspire (requires Docker)
dotnet test tests/Taskify.E2E.Tests
```

## Expected outcomes (acceptance summary)

- Five predefined users + three sample projects visible with no login (SC-002).
- Full create → assign → comment → move-to-Done flow completes in under 3 minutes (SC-001).
- Every task is in exactly one column; four columns always visible (SC-003).
- All data persists across refresh/restart (SC-004).
- 100% of empty/invalid input rejected cleanly, no crash/corruption (SC-005).
