# Tasks: Kanban Task Management

**Input**: Design documents from `/specs/001-kanban-task-management/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Test tasks are included because the constitution's *Development Workflow & Quality Gates* requires automated tests for security-relevant and validation paths (input validation is a non-negotiable principle).

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- .NET Aspire monorepo per plan.md: `src/Taskify.AppHost/`, `src/Taskify.ServiceDefaults/`, `src/Taskify.Api/`, `src/Taskify.Web/`
- Tests: `tests/Taskify.Api.Tests/`, `tests/Taskify.Web.Tests/`, `tests/Taskify.E2E.Tests/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [X] T001 Create the .NET 10 solution and project structure (`Taskify.sln`; `src/Taskify.AppHost`, `src/Taskify.ServiceDefaults`, `src/Taskify.Api` (Minimal API), `src/Taskify.Web` (Blazor Server); `tests/Taskify.Api.Tests`, `tests/Taskify.Web.Tests`, `tests/Taskify.E2E.Tests`)
- [X] T002 [P] Configure the Aspire AppHost to register `Taskify.Api`, `Taskify.Web`, and a PostgreSQL resource in `src/Taskify.AppHost/Program.cs`
- [X] T003 [P] Add shared service defaults (health checks, OpenTelemetry, resilience) in `src/Taskify.ServiceDefaults/Extensions.cs`
- [X] T004 [P] Configure OpenAPI/Swagger with XML doc comments in `src/Taskify.Api/Program.cs`
- [X] T005 [P] Add FluentValidation packages and register validators in `src/Taskify.Api/Program.cs`
- [X] T006 [P] Scaffold the three test projects with xUnit / bUnit / Aspire.Hosting.Testing references in `tests/*.csproj`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T007 Create EF Core entity models (User, Project, Task, Comment, Notification) and the `KanbanColumn` enum in `src/Taskify.Api/Models/`
- [X] T008 [P] Create request/response contract records in `src/Taskify.Api/Contracts/`
- [X] T009 Create `ApplicationDbContext` and the initial EF Core migration in `src/Taskify.Api/Data/` (depends on T007)
- [X] T010 [P] Implement seed data (five users, three sample projects) in `src/Taskify.Api/Data/SeedData.cs` (depends on T007, T009)
- [X] T011 [P] Implement FluentValidation validators for project/task/comment inputs (required, trimmed, length, enum) in `src/Taskify.Api/Validation/` (depends on T008)
- [X] T012 Implement the identity guard filter (validate `X-Taskify-User-Id` against the five users; reject otherwise) in `src/Taskify.Api/` (depends on T007, T009)
- [X] T013 [P] Implement the centralized error envelope helper (ValidationFailed/NotFound/Unauthorized shapes) in `src/Taskify.Api/`
- [X] T014 [P] Create the typed `ApiClient` and base HTTP configuration in `src/Taskify.Web/Services/ApiClient.cs`
- [X] T015 [P] Create the SignalR hub and client connection service in `src/Taskify.Web/Hubs/`

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Create Projects and Tasks (Priority: P1) 🎯 MVP

**Goal**: A user creates a project and adds tasks; new tasks start in "To Do".

**Independent Test**: Create a project, add a task, and see it appear in the project's "To Do" column.

### Implementation for User Story 1

- [X] T016 [US1] Implement `GET /api/projects` and `POST /api/projects` in `src/Taskify.Api/Endpoints/ProjectsEndpoints.cs`
- [X] T017 [US1] Implement `GET/POST /api/projects/{projectId}/tasks` and `GET /api/tasks/{taskId}` (new tasks start in `ToDo`) in `src/Taskify.Api/Endpoints/TasksEndpoints.cs`
- [X] T018 [US1] Implement the project list and "create project" UI in `src/Taskify.Web/Pages/`
- [X] T019 [US1] Implement the Kanban board page with four columns and the "add task" form in `src/Taskify.Web/Pages/` and `src/Taskify.Web/Components/`
- [X] T020 [US1] Wire the board page to `ApiClient` for loading/creating projects and tasks in `src/Taskify.Web/`
- [X] T021 [P] [US1] Add API tests for project/task creation validation (valid + empty/long input) in `tests/Taskify.Api.Tests/`

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - Move Tasks Across Kanban Columns (Priority: P2)

**Goal**: A user moves tasks between the four columns, including backward moves.

**Independent Test**: Drag a task from "To Do" to "Done" and confirm it appears in exactly one column.

### Implementation for User Story 2

- [X] T022 [US2] Implement `PATCH /api/tasks/{taskId}` for status changes (validate the enum; allow any column) in `src/Taskify.Api/Endpoints/TasksEndpoints.cs`
- [X] T023 [US2] Implement drag-and-drop between columns with a keyboard-accessible move fallback in `src/Taskify.Web/Components/`
- [X] T024 [US2] Broadcast `task.moved` over SignalR and update all connected clients in `src/Taskify.Web/Hubs/` and `src/Taskify.Web/Components/`
- [X] T025 [P] [US2] Add API tests for status-transition validation (invalid enum rejected) in `tests/Taskify.Api.Tests/`

**Checkpoint**: User Stories 1 AND 2 both work independently

---

## Phase 5: User Story 3 - Assign Tasks to Team Members (Priority: P3)

**Goal**: A user assigns (or clears) a task's assignee from the five predefined users.

**Independent Test**: Assign a task to an engineer and see that user shown as the assignee.

### Implementation for User Story 3

- [X] T026 [US3] Implement `GET /api/users` and assignment via `PATCH /api/tasks/{taskId}` (set/change/clear `assigneeId`) in `src/Taskify.Api/Endpoints/`
- [X] T027 [US3] Implement the assignee dropdown (five users) and assignment controls on task cards in `src/Taskify.Web/Components/`
- [X] T028 [US3] Broadcast `task.assigned` over SignalR in `src/Taskify.Web/Hubs/`
- [X] T029 [P] [US3] Add API tests for assignment validation (unknown assignee rejected; clearing works) in `tests/Taskify.Api.Tests/`

**Checkpoint**: User Stories 1, 2, AND 3 work independently

---

## Phase 6: User Story 4 - Comment on Tasks (Priority: P4)

**Goal**: A user adds a comment to a task and reads all comments chronologically.

**Independent Test**: Add a comment to a task and see it appear with author and time.

### Implementation for User Story 4

- [X] T030 [US4] Implement `POST/GET /api/tasks/{taskId}/comments` in `src/Taskify.Api/Endpoints/CommentsEndpoints.cs`
- [X] T031 [US4] Implement the comment thread UI (add form + chronological list) in `src/Taskify.Web/Components/`
- [X] T032 [US4] Broadcast `comment.added` over SignalR in `src/Taskify.Web/Hubs/`
- [X] T033 [P] [US4] Add API tests for comment validation (empty comment rejected) in `tests/Taskify.Api.Tests/`

**Checkpoint**: User Stories 1-4 work independently

---

## Phase 7: User Story 5 - Select Identity Without Login and View Sample Projects (Priority: P5)

**Goal**: A user selects their identity from the five predefined users (no login) and sees the three sample projects.

**Independent Test**: Open the app, select an identity, and confirm five users and three sample projects are available with no authentication.

### Implementation for User Story 5

- [X] T034 [US5] Implement the identity selection UI (choose from five users; persist the active selection) in `src/Taskify.Web/`
- [X] T035 [US5] Send the `X-Taskify-User-Id` header from `ApiClient` on all mutating requests in `src/Taskify.Web/Services/ApiClient.cs`
- [X] T036 [US5] Verify the three sample projects render on first open (seed data visible) in `src/Taskify.Web/Pages/`
- [X] T037 [P] [US5] Add API tests for the identity guard (missing/unknown header → 401) in `tests/Taskify.Api.Tests/`

**Checkpoint**: All five user stories are independently functional

---

## Phase 8: Notifications (Cross-cutting scope addition)

**Purpose**: In-app notifications added by the plan input (generated on assign/status-change/comment; exposed via REST + SignalR).

- [X] T038 Implement notification generation on assign, status-change, and comment events in `src/Taskify.Api/`
- [X] T039 Implement `GET /api/notifications` and `PATCH /api/notifications/{id}/read` in `src/Taskify.Api/Endpoints/NotificationsEndpoints.cs`
- [X] T040 Implement the notification list UI (unread badge) in `src/Taskify.Web/`
- [X] T041 Broadcast `notification.created` over SignalR in `src/Taskify.Web/Hubs/`
- [X] T042 [P] Add API tests for notifications (generation + mark-read) in `tests/Taskify.Api.Tests/`

---

## Phase 9: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [X] T043 [P] Add bUnit component tests for board rendering and drag/drop in `tests/Taskify.Web.Tests/`
- [X] T044 [P] Add an Aspire end-to-end smoke test in `tests/Taskify.E2E.Tests/`
- [X] T045 [P] Configure secret handling (Postgres connection string via Aspire parameter/user-secrets, never committed) in `src/Taskify.AppHost/`
- [X] T046 Ensure OpenAPI + XML docs cover all endpoints (documentation gate) across `src/Taskify.Api/`
- [X] T047 Run `quickstart.md` validation scenarios end-to-end and fix any gaps

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phases 3-7)**: All depend on Foundational phase completion
  - User stories can then proceed in parallel (if staffed) or sequentially in priority order (P1 → P5)
- **Notifications (Phase 8)**: Depends on US2/US3/US4 (the events it reacts to)
- **Polish (Phase 9)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Starts after Foundational - no dependencies on other stories
- **User Story 2 (P2)**: Starts after Foundational - reuses US1 task entities/endpoints but independently testable
- **User Story 3 (P3)**: Starts after Foundational - reuses US1 task PATCH endpoint; independently testable
- **User Story 4 (P4)**: Starts after Foundational - comments attach to US1 tasks; independently testable
- **User Story 5 (P5)**: Starts after Foundational - wires identity selection into the ApiClient from US1-4

### Within Each User Story

- API endpoints before UI wiring
- Validation wired into endpoints before UI calls them
- Tests (where present) may be written first and expected to fail before implementation
- Core implementation before SignalR broadcast
- Story complete before moving to next priority

### Parallel Opportunities

- All Setup tasks marked [P] can run in parallel after T001
- All Foundational tasks marked [P] can run in parallel (after their noted dependencies)
- Once Foundational completes, all five user stories can start in parallel (if staffed)
- Test tasks marked [P] within each story can run in parallel with each other
- Different user stories can be worked on in parallel by different team members

---

## Parallel Example: Foundational phase

```bash
# After entities (T007) + migration (T009) exist, launch in parallel:
Task: "Implement seed data (five users, three sample projects) in src/Taskify.Api/Data/SeedData.cs"
Task: "Implement FluentValidation validators ... in src/Taskify.Api/Validation/"
Task: "Implement the centralized error envelope helper ... in src/Taskify.Api/"
Task: "Create the typed ApiClient ... in src/Taskify.Web/Services/ApiClient.cs"
Task: "Create the SignalR hub and client connection service ... in src/Taskify.Web/Hubs/"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Test User Story 1 independently (create project + task in "To Do")
5. Deploy/demo if ready

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 → Test independently → Deploy/Demo (MVP!)
3. Add User Story 2 (move) → Test independently → Deploy/Demo
4. Add User Story 3 (assign) → Test independently → Deploy/Demo
5. Add User Story 4 (comment) → Test independently → Deploy/Demo
6. Add User Story 5 (identity) → Test independently → Deploy/Demo
7. Add Notifications → Polish → Final validation via quickstart.md

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1 (create projects/tasks)
   - Developer B: User Story 2 (move tasks)
   - Developer C: User Story 3 (assign tasks)
3. Stories complete and integrate independently

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to a specific user story for traceability
- Each user story should be independently completable and testable
- Verify tests fail before implementing (where TDD applies)
- Commit after each task or logical group
- Stop at any checkpoint to validate a story independently
- Avoid: vague tasks, same-file conflicts, cross-story dependencies that break independence

---

## Phase 10: Convergence

- [X] T048 Add a phase-2 authentication plan and security note (SECURITY.md) resolving the no-login conflict with Constitution "Authentication & authorization" per Constitution (contradicts)
- [X] T049 Implement the plan-named SignalR hub in src/Taskify.Web/Hubs/BoardHub.cs per plan D7, or document the RealtimeBus substitution in plan.md (partial)
- [X] T050 Replace EnsureCreated() with an EF Core migration and MigrateAsync() in src/Taskify.Api/Data/ per T009 (partial)
- [X] T051 Add bUnit tests for Board.razor rendering and drag/drop in tests/Taskify.Web.Tests/ per T043 (partial)
- [X] T052 Verify the browser board flow (drag/drop, assign, comment, notifications) per quickstart.md, or add a Web E2E test per T047 (partial)
- [X] T053 Document the data-at-rest encryption deferral for PostgreSQL per Constitution (data protection) (missing)
- [X] T054 Add dependency vulnerability scanning (dotnet list package --vulnerable) per Constitution (dependency hygiene) (missing)
- [X] T055 Remove the hardcoded dev fallback Postgres password from src/Taskify.Api/Program.cs per Constitution (secret handling) (contradicts)
