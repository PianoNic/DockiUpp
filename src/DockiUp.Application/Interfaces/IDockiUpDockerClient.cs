using Docker.DotNet;

namespace DockiUp.Application.Interfaces
{
    public interface IDockiUpDockerClient
    {
        DockerClient DockerClient { get; }
    }
}
