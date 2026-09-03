# API Contracts: Kanban Task Management

Phase 1 output. REST interface exposed by `Taskify.Api`. Base path `/api`. All responses are
JSON. Mutating requests require the `X-Taskify-User-Id` header identifying one of the five
predefined users (FR-014). Errors use a consistent shape (see "Error Handling").

The same contracts are exposed as OpenAPI/Swagger at runtime for documentation (constitution
Principle IV).

## Conventions

- `Id` fields are UUIDs.
- `Timestamp` fields are ISO 8601 UTC strings.
- `Status` is one of: `ToDo`, `InProgress`, `InReview`, `Done`.
- All request bodies are validated server-side; invalid input returns `400` with field errors.

## Users

### GET /api/users

List the five predefined users (identity selection + assignment dropdown).

**Response 200**:

```json
[
  { "id": "uuid", "name": "Ada Lovelace", "role": "ProductManager" },
  { "id": "uuid", "name": "Grace Hopper", "role": "Engineer" }
]
```

## Projects

### GET /api/projects

List all projects (includes the three seeded samples).

**Response 200**:

```json
[
  { "id": "uuid", "name": "Website Redesign", "createdById": "uuid", "createdAt": "2026-09-03T00:00:00Z" }
]
```

### GET /api/projects/{projectId}

Fetch one project. **404** if not found.

### POST /api/projects

Create a project (FR-004). Requires `X-Taskify-User-Id`.

**Request**:

```json
{ "name": "Mobile App Launch" }
```

**Response 201**: the created project (same shape as GET). **400** if `name` is empty or too long.

## Tasks

### GET /api/projects/{projectId}/tasks

List tasks for a project (optionally `?status=ToDo` to filter by column). Returns tasks with
assignee and comment count.

```json
[
  {
    "id": "uuid", "projectId": "uuid", "title": "Set up CI",
    "description": null, "status": "ToDo",
    "assigneeId": "uuid", "createdById": "uuid",
    "createdAt": "2026-09-03T00:00:00Z", "updatedAt": "2026-09-03T00:00:00Z"
  }
]
```

### POST /api/projects/{projectId}/tasks

Create a task (FR-005). New tasks start in `ToDo`. Requires `X-Taskify-User-Id`.

**Request**:

```json
{ "title": "Draft release notes", "description": "Optional detail" }
```

**Response 201**: the created task. **400** if `title` is empty/too long or the project is
missing (**404**).

### GET /api/tasks/{taskId}

Fetch one task with its comments. **404** if not found.

### PATCH /api/tasks/{taskId}

Update task fields: `title`, `description`, `assigneeId` (assign/reassign/clear), and/or
`status` (move between columns) (FR-008, FR-009). Requires `X-Taskify-User-Id`. Only supplied
fields change. Setting `assigneeId` to `null` clears the assignment.

**Request** (partial; any subset):

```json
{ "status": "InProgress", "assigneeId": "uuid", "title": "New title" }
```

**Response 200**: the updated task. **400** on invalid enum/assignee. **404** if task missing.

### POST /api/tasks/{taskId}/comments

Add a comment (FR-010). Requires `X-Taskify-User-Id`.

**Request**:

```json
{ "text": "Blocked on design review" }
```

**Response 201**: the created comment with `authorId` and `createdAt`. **400** if `text` empty.

### GET /api/tasks/{taskId}/comments

List comments in chronological order (FR-011). **404** if task missing.

```json
[
  { "id": "uuid", "taskId": "uuid", "authorId": "uuid", "text": "Blocked on design review", "createdAt": "2026-09-03T00:00:00Z" }
]
```

## Notifications

### GET /api/notifications?userId={userId}

List notifications for a user, newest first (optionally `?unreadOnly=true`).

```json
[
  { "id": "uuid", "userId": "uuid", "type": "TaskAssigned", "message": "You were assigned \"Draft release notes\"",
    "taskId": "uuid", "projectId": "uuid", "isRead": false, "createdAt": "2026-09-03T00:00:00Z" }
]
```

### PATCH /api/notifications/{notificationId}/read

Mark one notification read (`{ "isRead": true }`). **Response 200**: updated notification.
**404** if not found.

## Error Handling

Consistent error envelope for all `4xx`/`5xx`:

```json
{ "error": { "code": "ValidationFailed", "message": "One or more fields are invalid", "fields": { "name": ["Name is required"] } } }
```

- `400 ValidationFailed` — input failed server-side validation (fail closed).
- `404 NotFound` — referenced entity does not exist.
- `401 Unauthorized` — `X-Taskify-User-Id` missing or not one of the five users (phase-1 guard).

## Real-Time (SignalR) Events

Published by `Taskify.Web` to connected clients after each successful mutation (D7). Event
names and payloads:

| Event | Payload |
|-------|---------|
| `task.moved` | `{ taskId, projectId, status }` |
| `task.assigned` | `{ taskId, assigneeId }` |
| `task.created` | `{ taskId, projectId }` |
| `comment.added` | `{ commentId, taskId }` |
| `notification.created` | `{ notificationId, userId }` |
