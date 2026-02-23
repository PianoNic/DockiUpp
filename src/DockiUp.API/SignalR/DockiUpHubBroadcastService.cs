using DockiUp.Application.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DockiUp.API.SignalR;

/// <summary>Broadcasts project/container data to all connected SignalR clients so the frontend updates without calling GetProjects.</summary>
public interface IDockiUpHubBroadcastService
{
    Task BroadcastContainersChangedAsync(ProjectDto[] projects, CancellationToken cancellationToken = default);
}

public class DockiUpHubBroadcastService : IDockiUpHubBroadcastService
{
    private readonly IHubContext<DockiUpHub> _hubContext;

    public DockiUpHubBroadcastService(IHubContext<DockiUpHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task BroadcastContainersChangedAsync(ProjectDto[] projects, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.All.SendAsync(DockiUpHubMessages.ContainersChanged, projects, cancellationToken);
    }
}
