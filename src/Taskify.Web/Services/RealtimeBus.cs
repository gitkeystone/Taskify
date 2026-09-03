namespace Taskify.Web.Services;

/// <summary>
/// In-process real-time event bus. Blazor Server's SignalR circuit carries re-renders to
/// connected browsers, so publishing an event here broadcasts updates to all clients.
/// </summary>
/// <remarks>
/// Substitutes the plan's "SignalR hub" decision (research D7) with the Blazor-Server-native
/// equivalent: a custom SignalR hub would require browser-side JS interop, whereas this bus
/// leverages the per-client Blazor circuit (SignalR under the hood) for the same real-time
/// cross-client behavior with less surface area. A dedicated hub can be introduced later if
/// the API must push events to clients independent of Web-initiated mutations.
/// </remarks>
public sealed class RealtimeBus
{
    public event Action? Changed;

    public void NotifyChanged() => Changed?.Invoke();
}
