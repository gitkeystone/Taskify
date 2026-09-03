namespace Taskify.Web.Services;

/// <summary>
/// Holds the currently selected identity (no-login phase 1) and the cached user list.
/// Scoped to the Blazor Server circuit, so identity persists per browser tab.
/// </summary>
public sealed class IdentityState
{
    public Guid? CurrentUserId { get; private set; }
    public string CurrentUserName { get; private set; } = string.Empty;
    public List<UserDto> Users { get; private set; } = [];

    public event Action? Changed;

    public void SetUsers(IEnumerable<UserDto> users)
    {
        Users = users.ToList();
    }

    public void SetUser(UserDto? user)
    {
        CurrentUserId = user?.Id;
        CurrentUserName = user?.Name ?? string.Empty;
        Changed?.Invoke();
    }

    public UserDto? CurrentUser => Users.FirstOrDefault(u => u.Id == CurrentUserId);
}
