namespace DockiUp.Domain
{
    /// <summary>An audit record of an action taken in DockiUp (deploy, start, stop, restart, update…).
    /// Written by <c>IActivityLogger</c> from command handlers and background jobs.</summary>
    public class ActivityEntry : BaseEntity
    {
        public required string Action { get; init; }
        public required string Target { get; init; }
        public Guid? ProjectId { get; init; }
        public string? Details { get; init; }
        /// <summary>Who triggered the action; null for background jobs (the UI renders "system").</summary>
        public string? ActorName { get; init; }
    }
}
