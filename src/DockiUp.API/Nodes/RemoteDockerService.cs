using DockiUp.Application.Dtos;
using DockiUp.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DockiUp.API.Nodes
{
    /// <summary>An <see cref="IDockerService"/> whose calls run on a remote node. Each method invokes
    /// the matching handler on the node's live SignalR connection; the node executes it against its own
    /// Docker daemon (and its own project folders) and returns the result. Void operations map to a
    /// bool result because SignalR client-result invocations must return a value.</summary>
    public class RemoteDockerService(Guid nodeId, IHubContext<NodeHub> hub, INodeRegistry registry) : IDockerService
    {
        private ISingleClientProxy Node()
        {
            if (!registry.TryGetConnectionId(nodeId, out var connectionId))
                throw new NodeOfflineException(nodeId);
            return hub.Clients.Client(connectionId);
        }

        public Task<ProjectDto[]> GetProjectsAsync()
            => Node().InvokeAsync<ProjectDto[]>("GetProjects", CancellationToken.None);

        public Task<ProjectDto?> GetProjectByDockerNameAsync(string dockerProjectName)
            => Node().InvokeAsync<ProjectDto?>("GetProjectByDockerName", dockerProjectName, CancellationToken.None);

        public Task<ContainerDto?> InspectContainerAsync(string containerId, CancellationToken cancellationToken = default)
            => Node().InvokeAsync<ContainerDto?>("InspectContainer", containerId, cancellationToken);

        public Task StartProjectAsync(string folderPath)
            => Node().InvokeAsync<bool>("StartProject", folderPath, CancellationToken.None);

        public Task StopProjectAsync(string folderPath)
            => Node().InvokeAsync<bool>("StopProject", folderPath, CancellationToken.None);

        public Task RestartProjectAsync(string folderPath)
            => Node().InvokeAsync<bool>("RestartProject", folderPath, CancellationToken.None);

        public Task StartContainerAsync(string containerId)
            => Node().InvokeAsync<bool>("StartContainer", containerId, CancellationToken.None);

        public Task StopContainerAsync(string containerId)
            => Node().InvokeAsync<bool>("StopContainer", containerId, CancellationToken.None);

        public Task RestartContainerAsync(string containerId)
            => Node().InvokeAsync<bool>("RestartContainer", containerId, CancellationToken.None);

        public Task<string> GetContainerLogsAsync(string containerId, int? tail = null, CancellationToken cancellationToken = default)
            => Node().InvokeAsync<string>("GetContainerLogs", containerId, tail, cancellationToken);
    }
}
