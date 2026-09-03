# Implementation Plan: Kanban Task Management

**Branch**: `001-kanban-task-management` | **Date**: 2026-09-03 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/001-kanban-task-management/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Taskify is a team productivity platform where five predefined users (one product manager, four
engineers) manage work on a Kanban board without login. Users create projects and tasks, move
tasks across four columns (To Do, In Progress, In Review, Done), assign tasks to team members,
and comment on tasks.

**Technical approach**: a distributed .NET 10 LTS solution orchestrated by **.NET Aspire**,
split into two independently deployable services — a **REST API** (system of record that owns
**PostgreSQL**) and a **Blazor Server** front end (drag-and-drop board + real-time updates via
SignalR). The Blazor app never touches the database directly; it talks to the API over
versioned REST contracts, satisfying the constitution's microservices rule.

> **Scope change from spec**: the plan input adds a **notifications** REST API. The spec's
> Assumptions still mark "notifications, email, deadlines, and reporting" as out of scope; this
> plan brings in-app **notifications** into scope while email/reporting remain out of scope.

## Technical Context

**Language/Version**: C# 14 / .NET 10 LTS

**Primary Dependencies**: .NET Aspire 13 (orchestration), ASP.NET Core Minimal APIs + Blazor
Server (Interactive Server) + SignalR, EF Core 10 with Npgsql, FluentValidation

**Storage**: PostgreSQL (managed via the Aspire `AddPostgres` resource; EF Core migrations)

**Testing**: xUnit (API unit/integration), bUnit (Blazor components), Aspire.Hosting.Testing
(end-to-end), WebApplicationFactory (API integration)

**Target Platform**: Linux containers (Docker); local development via the Aspire AppHost and
dashboard

**Project Type**: distributed web application (microservices + Blazor Server UI)

**Performance Goals**: board interactions (create/move/assign/comment) complete in under 1
second at p95; real-time board updates propagate to connected clients in under 500 ms

**Constraints**: Security-first — all API inputs validated server-side and rejected on invalid
input; no login (identity selected from five predefined users); all public interfaces
documented (OpenAPI + XML docs)

**Scale/Scope**: 5 users, 3+ sample projects, small-team single-instance MVP (no horizontal
scaling in this phase)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **I. Security-First** — PASS (with documented deviation): security is the top constraint;
  validation and fail-closed behavior are first-class (see Complexity Tracking for the no-login
  phase-1 deviation).
- **II. Input Validation** — PASS: FluentValidation on every API input; server-side only;
  client-side validation is UX-only; trim + length + required rules; fail closed.
- **III. Microservices Architecture** — PASS: `Taskify.Api` and `Taskify.Web` are independently
  deployable; the Web communicates with the Api only over REST (no shared direct DB access);
  contracts are versioned under `specs/.../contracts/`.
- **IV. Comprehensive Documentation** — PASS: OpenAPI for the API, XML doc comments, this plan,
  data model, and quickstart.
- **Security Requirements — Secret handling** — PASS: DB connection strings and secrets managed
  via Aspire parameters/secret stores, never committed.
- **Security Requirements — Authentication & authorization** — DEVIATION (phase-1): no login;
  identity is client-selected. Justified in Complexity Tracking; real auth is deferred.
- **Security Requirements — Data protection** — PASS for transit (HTTPS); at-rest encryption
  noted as a deployment follow-up.

## Project Structure

### Documentation (this feature)

```text
specs/001-kanban-task-management/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md        # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
├── contracts/           # Phase 1 output (/speckit-plan command)
│   └── api-contracts.md
└── tasks.md             # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
Taskify.sln
src/
├── Taskify.AppHost/          # .NET Aspire orchestrator (defines services + Postgres)
├── Taskify.ServiceDefaults/  # Shared resilience, health checks, OpenTelemetry defaults
├── Taskify.Api/              # REST API microservice (owns PostgreSQL)
│   ├── Endpoints/            # Minimal API route groups (projects, tasks, notifications, users)
│   ├── Models/               # EF Core entities + DTOs
│   ├── Validation/           # FluentValidation validators
│   ├── Data/                 # DbContext, migrations, seed data (5 users, 3 projects)
│   └── Contracts/            # Request/response records (shared with tests)
└── Taskify.Web/              # Blazor Server front end
    ├── Components/           # Board, columns, task cards, drag-and-drop
    ├── Pages/                # Project list, board, task detail, notifications
    ├── Services/             # ApiClient (typed HTTP client), real-time (SignalR) hub
    └── Hubs/                 # SignalR hub for real-time board/notification updates

tests/
├── Taskify.Api.Tests/        # xUnit unit + integration tests (validation, endpoints)
├── Taskify.Web.Tests/        # bUnit component tests (drag/drop, rendering)
└── Taskify.E2E.Tests/        # Aspire end-to-end tests
```

**Structure Decision**: Monorepo with an Aspire `AppHost` orchestrating two services
(`Taskify.Api`, `Taskify.Web`) and a Postgres resource. `Taskify.Api` is the system of record
and the only service with database access; `Taskify.Web` renders the UI and calls the API via a
typed HTTP client, pushing real-time updates over SignalR.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| No authentication/authorization (identity is client-selected) | Explicit phase-1 product requirement ("no login for this first phase") | Requiring login would violate the stated MVP scope; real auth is deferred to a later phase |
| Notifications in scope (spec marked them out of scope) | Plan input explicitly requires a notifications REST API | Omitting notifications would ignore the stated architecture direction |
