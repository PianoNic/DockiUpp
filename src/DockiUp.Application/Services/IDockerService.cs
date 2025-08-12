using DockiUp.Application.Dtos;

namespace DockiUp.Application.Services
{
    public interface IDockerService
    {
        Task<Guid> SetupDirectory(SetupContainerDto setupContainerDto);
        Task PullAsync(Guid containerId);
        Task StartAsync(Guid containerId);
        Task StopAsync(Guid containerId);
    }
}
