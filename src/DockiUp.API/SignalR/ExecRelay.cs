using System.Collections.Concurrent;

namespace DockiUp.API.SignalR
{
    /// <summary>Maps a node-hosted exec session to the node running it and the browser connection that
    /// opened it, so the browser hub can route input to the node and the node hub can forward the node's
    /// output back to the right browser.</summary>
    public interface IExecRelay
    {
        void RegisterExec(string sessionId, Guid nodeId, string browserConnectionId);
        bool TryGetExec(string sessionId, out Guid nodeId, out string browserConnectionId);
        void RemoveExec(string sessionId);
    }

    public sealed class ExecRelay : IExecRelay
    {
        private readonly ConcurrentDictionary<string, (Guid NodeId, string BrowserConnectionId)> _sessions = new();

        public void RegisterExec(string sessionId, Guid nodeId, string browserConnectionId)
            => _sessions[sessionId] = (nodeId, browserConnectionId);

        public bool TryGetExec(string sessionId, out Guid nodeId, out string browserConnectionId)
        {
            if (_sessions.TryGetValue(sessionId, out var entry))
            {
                nodeId = entry.NodeId;
                browserConnectionId = entry.BrowserConnectionId;
                return true;
            }
            nodeId = default;
            browserConnectionId = string.Empty;
            return false;
        }

        public void RemoveExec(string sessionId) => _sessions.TryRemove(sessionId, out _);
    }
}
