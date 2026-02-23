namespace DockiUp.API.SignalR;

/// <summary>Client-side contract for SignalR messages (method names).</summary>
public static class DockiUpHubMessages
{
    /// <summary>Emitted when projects/containers state changed (e.g. after start/stop/restart or crash).</summary>
    public const string ContainersChanged = "ContainersChanged";
}
