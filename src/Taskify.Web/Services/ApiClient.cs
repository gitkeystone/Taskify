using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Taskify.Web.Services;

/// <summary>Typed HTTP client for the Taskify REST API.</summary>
public sealed class ApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _http;
    private readonly IdentityState _identity;

    public ApiClient(HttpClient http, IdentityState identity)
    {
        _http = http;
        _identity = identity;
    }

    public async Task<List<UserDto>> GetUsersAsync(CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<List<UserDto>>("/api/users", JsonOptions, ct) ?? [];

    public async Task<List<ProjectDto>> GetProjectsAsync(CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<List<ProjectDto>>("/api/projects", JsonOptions, ct) ?? [];

    public async Task<ProjectDto> CreateProjectAsync(string name, CancellationToken ct = default) =>
        await SendAsync<CreateProjectRequest, ProjectDto>(HttpMethod.Post, "/api/projects", new CreateProjectRequest(name), ct);

    public async Task<List<TaskDto>> GetTasksAsync(Guid projectId, CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<List<TaskDto>>($"/api/projects/{projectId}/tasks", JsonOptions, ct) ?? [];

    public async Task<TaskDto> CreateTaskAsync(Guid projectId, string title, string? description, CancellationToken ct = default) =>
        await SendAsync<CreateTaskRequest, TaskDto>(HttpMethod.Post, $"/api/projects/{projectId}/tasks", new CreateTaskRequest(title, description), ct);

    public async Task<TaskDto> UpdateTaskAsync(Guid taskId, UpdateTaskRequest update, CancellationToken ct = default) =>
        await SendAsync<UpdateTaskRequest, TaskDto>(HttpMethod.Patch, $"/api/tasks/{taskId}", update, ct);

    public async Task<List<CommentDto>> GetCommentsAsync(Guid taskId, CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<List<CommentDto>>($"/api/tasks/{taskId}/comments", JsonOptions, ct) ?? [];

    public async Task<CommentDto> AddCommentAsync(Guid taskId, string text, CancellationToken ct = default) =>
        await SendAsync<AddCommentRequest, CommentDto>(HttpMethod.Post, $"/api/tasks/{taskId}/comments", new AddCommentRequest(text), ct);

    public async Task<List<NotificationDto>> GetNotificationsAsync(Guid userId, CancellationToken ct = default) =>
        await _http.GetFromJsonAsync<List<NotificationDto>>($"/api/notifications?userId={userId}", JsonOptions, ct) ?? [];

    public async Task<NotificationDto> MarkNotificationReadAsync(Guid notificationId, CancellationToken ct = default) =>
        await SendAsync<MarkReadRequest, NotificationDto>(HttpMethod.Patch, $"/api/notifications/{notificationId}/read", new MarkReadRequest(true), ct);

    private async Task<TResponse> SendAsync<TRequest, TResponse>(HttpMethod method, string uri, TRequest body, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(method, uri)
        {
            Content = JsonContent.Create(body, options: JsonOptions)
        };

        if (_identity.CurrentUserId is { } id)
        {
            request.Headers.Add("X-Taskify-User-Id", id.ToString());
        }

        using var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, ct))!;
    }
}
