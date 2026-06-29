using System.Collections.Concurrent;
using DockiUp.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DockiUp.API.SignalR;

/// <summary>Browser-facing hub. Hosts interactive container exec (web terminal): the browser calls
/// <c>StartExec</c>/<c>WriteExec</c>/<c>ResizeExec</c>/<c>EndExec</c> and receives <c>ExecOutput</c>/
/// <c>ExecExited</c> back.</summary>
public class DockiUpHub(IContainerExecRegistry registry, IHubContext<DockiUpHub> hubContext) : Hub
{
    // Live exec sessions per connection, so a closed tab tears down its docker exec sessions.
    private static readonly ConcurrentDictionary<string, HashSet<string>> _sessionsByConnection = new();

    public async Task<string> StartExec(string containerId, uint cols, uint rows)
    {
        var session = await registry.StartAsync(containerId, cols == 0 ? 120 : cols, rows == 0 ? 30 : rows, Context.ConnectionAborted);

        // The Hub instance is disposed when this method returns, so Clients.Caller becomes a dead
        // proxy by the time the shell echoes its first byte. Route via IHubContext + explicit
        // connectionId so callbacks remain valid for the lifetime of the session.
        var connectionId = Context.ConnectionId;
        var client = hubContext.Clients.Client(connectionId);

        session.Output += async data =>
        {
            try { await client.SendAsync("ExecOutput", session.Id, Convert.ToBase64String(data.Span)); }
            catch { /* connection gone */ }
        };
        session.Exited += async code =>
        {
            try { await client.SendAsync("ExecExited", session.Id, code); } catch { }
            if (_sessionsByConnection.TryGetValue(connectionId, out var set))
            {
                lock (set) set.Remove(session.Id);
            }
        };

        var sessions = _sessionsByConnection.GetOrAdd(connectionId, _ => new HashSet<string>());
        lock (sessions) sessions.Add(session.Id);

        return session.Id;
    }

    public async Task WriteExec(string sessionId, string base64Data)
    {
        var session = registry.Get(sessionId);
        if (session is null) return;
        await session.WriteAsync(Convert.FromBase64String(base64Data), Context.ConnectionAborted);
    }

    public async Task ResizeExec(string sessionId, uint cols, uint rows)
    {
        var session = registry.Get(sessionId);
        if (session is null) return;
        await session.ResizeAsync(cols, rows, Context.ConnectionAborted);
    }

    public async Task EndExec(string sessionId)
    {
        if (_sessionsByConnection.TryGetValue(Context.ConnectionId, out var set))
        {
            lock (set) set.Remove(sessionId);
        }
        await registry.EndAsync(sessionId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_sessionsByConnection.TryRemove(Context.ConnectionId, out var set))
        {
            string[] ids;
            lock (set) ids = set.ToArray();
            foreach (var id in ids)
            {
                try { await registry.EndAsync(id); } catch { }
            }
        }
        await base.OnDisconnectedAsync(exception);
    }
}
