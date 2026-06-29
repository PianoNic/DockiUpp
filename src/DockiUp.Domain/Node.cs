namespace DockiUp.Domain
{
    /// <summary>A worker node registered with this control plane. A node authenticates with a
    /// pre-shared token; we store only its SHA-256 hash (<see cref="TokenHash"/>) and derive the
    /// node's identity from it on connect, so the node never needs to know its own Id. Rows can be
    /// created up front from the "Add node" UI and stay pending (empty runtime fields) until the node
    /// actually dials in and registers. Online state is NOT stored; it's derived from the live SignalR
    /// registry. CreatedAt is the first-seen time.</summary>
    public class Node : BaseEntity
    {
        public required string Name { get; set; }

        /// <summary>SHA-256 (base64) of the node's pre-shared token.</summary>
        public string? TokenHash { get; set; }

        // Runtime details reported by the node on registration. Empty until it first connects.
        public string MachineName { get; set; } = "";
        public string Os { get; set; } = "";
        public string DockerVersion { get; set; } = "";

        public DateTime LastSeenAt { get; set; } = DateTime.UtcNow;
    }
}
