using DockiUp.Application.Dtos;

namespace DockiUp.Application.Interfaces
{
    public interface IDockerService
    {
        Task<ProjectDto[]> GetProjectsAsync();
        Task StartProjectAsync(string folderPath);
        Task StopProjectAsync(string folderPath);
        Task RestartProjectAsync(string folderPath);

        Task StartContainerAsync(string containerId);
        Task StopContainerAsync(string containerId);
        Task RestartContainerAsync(string containerId);
    }
}
