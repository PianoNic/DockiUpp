using Docker.DotNet;
using DockiUp.Application.Interfaces;

namespace DockiUp.Infrastructure.Clients
{
    public class DockiUpDockerClient : IDockiUpDockerClient
    {
        public DockerClient DockerClient { get; }

        DockerClient IDockiUpDockerClient.DockerClient => DockerClient;

        public DockiUpDockerClient()
        {
            var config = new DockerClientConfiguration();
            DockerClient = config.CreateClient();
        }
    }
}
