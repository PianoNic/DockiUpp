namespace DockiUp.Application.Interfaces
{
    /// <summary>Lists the nodes currently connected to the control plane. Implemented in the API layer
    /// (which owns the live-connection registry); the Application layer depends only on this abstraction
    /// so query handlers can fan out to online nodes. On a node process this returns nothing.</summary>
    public interface INodeDirectory
    {
        IReadOnlyList<Guid> GetOnlineNodeIds();
    }
}
