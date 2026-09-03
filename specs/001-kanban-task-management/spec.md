# Feature Specification: Kanban Task Management

**Feature Branch**: `001-kanban-task-management`

**Created**: 2026-09-03

**Status**: Draft

**Input**: User description: "Develop Taskify, a team productivity platform where predefined users create projects, assign tasks, comment, and move tasks across Kanban columns (To Do, In Progress, In Review, Done). Five users (one product manager, four engineers), three sample projects, no login for this first phase."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Create Projects and Tasks (Priority: P1)

A predefined team member opens Taskify, selects their identity (no login required), and
creates a project. Within that project they add tasks, and each new task starts in the
"To Do" column.

**Why this priority**: Projects and tasks are the foundation of the platform. Without them no
other action (assigning, commenting, or moving) is possible, and the platform delivers no value.

**Independent Test**: Can be fully tested by creating a project and adding one task, which
delivers a visible project board with a task in "To Do".

**Acceptance Scenarios**:

1. **Given** the app is open and a user has selected an identity, **When** they create a
   project with a valid name, **Then** the project appears in the project list.
2. **Given** a project exists, **When** a user adds a task with a valid title, **Then** the
   task appears in the project's "To Do" column.
3. **Given** a project form with an empty name, **When** the user submits, **Then** the system
   rejects the submission with a clear error and no project is created.

---

### User Story 2 - Move Tasks Across Kanban Columns (Priority: P2)

A team member moves tasks through the four Kanban columns (To Do, In Progress, In Review,
Done) to reflect the current state of the work.

**Why this priority**: Moving tasks across columns is the core Kanban workflow and the primary
reason the platform uses a board rather than a plain list.

**Independent Test**: Can be fully tested by moving one task from "To Do" to "Done", which
visibly updates the board and persists the new column.

**Acceptance Scenarios**:

1. **Given** a task exists in "To Do", **When** a user moves it to "In Progress", **Then** the
   task disappears from "To Do" and appears in "In Progress".
2. **Given** a task in any column, **When** a user moves it to another column, **Then** the
   task appears in exactly one column (the destination) and never in two columns at once.
3. **Given** a task in "Done", **When** a user moves it back to "In Progress", **Then** the
   task returns to "In Progress" (backward moves are allowed).

---

### User Story 3 - Assign Tasks to Team Members (Priority: P3)

A team member assigns a task to one of the five predefined users so ownership is clear.

**Why this priority**: Assignment communicates who is responsible for a task, which is
essential for team coordination once tasks exist and move across the board.

**Independent Test**: Can be fully tested by assigning a task to one of the five predefined
users, which then shows that user as the assignee on the task.

**Acceptance Scenarios**:

1. **Given** a task with no assignee, **When** a user assigns it to a predefined team member,
   **Then** that member is shown as the task's assignee.
2. **Given** a task already assigned, **When** a user reassigns it to a different predefined
   member, **Then** the assignee updates to the newly selected member.
3. **Given** the assignment options, **When** a user views them, **Then** only the five
   predefined users (one product manager and four engineers) are available as assignees.

---

### User Story 4 - Comment on Tasks (Priority: P4)

A team member adds a comment to a task and reads comments left by others to discuss the work.

**Why this priority**: Comments enable team discussion and context sharing, which becomes
valuable after tasks are created, moved, and assigned.

**Independent Test**: Can be fully tested by adding one comment to a task, which then appears
in the task's comment thread alongside its author.

**Acceptance Scenarios**:

1. **Given** a task exists, **When** a user adds a comment with non-empty text, **Then** the
   comment appears on the task with the author and time recorded.
2. **Given** a task with comments, **When** a user opens the task, **Then** all comments are
   visible in chronological order.
3. **Given** an empty comment, **When** a user submits it, **Then** the system rejects it with
   a clear error and no comment is added.

---

### User Story 5 - Select Identity Without Login and View Sample Projects (Priority: P5)

On first use, a team member selects which of the five predefined users they are acting as, and
immediately sees the three sample projects without any login or setup.

**Why this priority**: This defines the no-login entry experience and confirms the predefined
data is available out of the box, enabling the other stories to be demonstrated immediately.

**Independent Test**: Can be fully tested by opening the app, selecting an identity, and
confirming all five users and three sample projects are present with no authentication.

**Acceptance Scenarios**:

1. **Given** the app is opened for the first time, **When** no user is logged in, **Then** the
   user is offered the five predefined identities to choose from (no credentials required).
2. **Given** a user selects an identity, **When** they view the project list, **Then** the
   three sample projects are visible.
3. **Given** a user has selected an identity, **When** they perform any action (create, assign,
   comment, move), **Then** that action is attributed to the selected identity.

---

### Edge Cases

- What happens when a project name or task title is empty, whitespace-only, or excessively
  long? (Rejected with a clear, non-crashing validation message.)
- What happens when a comment is empty or whitespace-only? (Rejected; no comment is saved.)
- Can a task be moved in any direction, including backward (e.g., Done → In Progress)? (Yes;
  movement between any two columns is allowed.)
- Can a task exist without an assignee? (Yes; tasks start unassigned and may remain so.)
- What happens to a task's comments and assignee when the task is moved? (They follow the
  task, since they belong to the task, not the column.)
- What happens when two projects share the same name? (Allowed in this phase; names are
  required but not unique.)
- What happens when a user submits input containing leading/trailing whitespace? (Input is
  trimmed before validation and storage.)

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST provide exactly five predefined users: one product manager and four
  engineers.
- **FR-002**: System MUST require no login; the active user selects their identity from the
  five predefined users.
- **FR-003**: System MUST come preloaded with three sample projects.
- **FR-004**: Users MUST be able to create a new project with a non-empty name.
- **FR-005**: Users MUST be able to create a task within a project with a non-empty title.
- **FR-006**: Each task MUST belong to exactly one of the four Kanban columns ("To Do", "In
  Progress", "In Review", "Done") at any given time.
- **FR-007**: A newly created task MUST start in the "To Do" column.
- **FR-008**: Users MUST be able to move a task from its current column to any of the other
  three columns.
- **FR-009**: Users MUST be able to assign a task to one of the five predefined users, and to
  change or clear the assignment.
- **FR-010**: Users MUST be able to add a non-empty comment to a task.
- **FR-011**: Users MUST be able to view all comments on a task in chronological order.
- **FR-012**: System MUST validate all user input (project names, task titles, comments) and
  reject invalid or empty input with a clear error message without crashing or corrupting data.
- **FR-013**: System MUST persist projects, tasks, assignments, comments, and Kanban column
  positions so they survive a refresh or restart.
- **FR-014**: All create, assign, comment, and move actions MUST be attributed to the currently
  selected predefined user.

### Key Entities

- **User**: A predefined team member. Attributes: name and role (product manager or engineer).
  There are exactly five instances in this phase.
- **Project**: A container for tasks. Attributes: name and creator. The system ships with three
  sample projects.
- **Task**: A unit of work. Attributes: title, optional description, current Kanban column,
  optional assignee, and a list of comments. Belongs to exactly one project.
- **Comment**: A text note on a task. Attributes: text, author, and timestamp. Belongs to
  exactly one task.
- **Kanban Column**: One of the fixed, ordered set "To Do", "In Progress", "In Review", "Done".
  A task occupies exactly one column at a time.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A predefined user can select an identity, create a project, add a task, assign it,
  and move it to "Done" in under 3 minutes on first use.
- **SC-002**: All five predefined users and three sample projects are visible immediately on
  first open, with no login or setup required.
- **SC-003**: 100% of tasks appear in exactly one Kanban column at all times, and the four
  columns are always visible on a project board.
- **SC-004**: 100% of projects, tasks, assignments, and comments persist after a refresh or
  restart and are visible to all team members.
- **SC-005**: 100% of empty or invalid input is rejected with a clear message, and no invalid
  input corrupts data or causes the system to crash.

## Assumptions

- No authentication or authorization exists in this first phase; a user identifies themselves
  by selecting one of the five predefined identities. All five users share the same view and
  permissions (team-wide visibility).
- Task creation is in scope even though the description lists "create projects, assign tasks,
  comment, and move tasks" — tasks must exist before they can be assigned, commented on, or
  moved.
- Kanban moves are allowed in any direction between columns (not restricted to forward-only).
- Assignment is optional; a task may be unassigned, and assignments can be changed or cleared.
- Task titles and project names are required; a longer optional task description is supported.
- Duplicate project names are permitted in this phase (names are required but not unique).
- Data is persisted across sessions rather than held in memory only.
- Comments are plain text; rich formatting, attachments, and mentions are out of scope.
- Notifications, email, deadlines, and reporting are out of scope for this phase.
- Mobile/native support is out of scope for this phase.
