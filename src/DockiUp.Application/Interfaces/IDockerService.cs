using DockiUp.Application.Dtos;

namespace DockiUp.Application.Interfaces
{
    /// <summary>
    /// Docker and compose operations (Komodo-style: projects/stacks and container lifecycle).
    /// </summary>
    public interface IDockerService
    {
        Task<ProjectDto[]> GetProjectsAsync();

        /// <summary>Daemon-only project listing: no database reconciliation (Id/ManagedByDockiUp left
        /// unset). Safe to run on a node, which has no application database; the control plane reconciles
        /// the returned projects against its own records.</summary>
        Task<ProjectDto[]> GetRawProjectsAsync();

        Task<ProjectDto?> GetProjectByDockerNameAsync(string dockerProjectName);
        Task<ContainerDto?> InspectContainerAsync(string containerId, CancellationToken cancellationToken = default);

        Task StartProjectAsync(string folderPath);
        Task StopProjectAsync(string folderPath);
        Task RestartProjectAsync(string folderPath);

        Task StartContainerAsync(string containerId);
        Task StopContainerAsync(string containerId);
        Task RestartContainerAsync(string containerId);

        /// <summary>
        /// Get container logs (stdout + stderr). Returns decoded text.
        /// </summary>
        Task<string> GetContainerLogsAsync(string containerId, int? tail = null, CancellationToken cancellationToken = default);
    }
}
