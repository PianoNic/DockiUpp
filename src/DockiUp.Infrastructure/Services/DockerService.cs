using System.Text;
using Docker.DotNet.Models;
using DockiUp.Application.Dtos;
using DockiUp.Application.Interfaces;
using DockiUp.Application.Mappers;
using System.Diagnostics;

namespace DockiUp.Infrastructure.Services
{
    public class DockerService : IDockerService
    {
        private readonly IDockiUpDockerClient _dockiUpDockerClient;
        private readonly IDockiUpDbContext _dbContext;
        private const string ComposeFileName = "dockiup_compose.yml";
        public DockerService(IDockiUpDockerClient dockiUpDockerClient, IDockiUpDbContext dbContext)
        {
            _dockiUpDockerClient = dockiUpDockerClient;
            _dbContext = dbContext;
        }

        public async Task<ProjectDto[]> GetProjectsAsync()
        {
            var containers = await _dockiUpDockerClient.DockerClient.Containers
                .ListContainersAsync(new ContainersListParameters { All = true });

            var dbContainers = _dbContext.ProjectInfo.ToDictionary(a => a.DockerProjectName);

            return containers
                .Select(container =>
                    {
                        container.Labels.TryGetValue("com.docker.compose.project", out string? projectName);
                        container.Labels.TryGetValue("com.docker.compose.service", out string? serviceName);

                        if (string.IsNullOrEmpty(projectName))
                            return (ContainerDto?)null;

                        return new ContainerDto
                        {
                            Id = container.ID,
                            Name = container.Names.SingleOrDefault()?.TrimStart('/') ?? string.Empty,
                            Status = container.Status,
                            State = container.State.ToEnum(),
                            ProjectName = projectName,
                            ServiceName = serviceName ?? string.Empty
                        };
                    }
                )
                .Where(dto => dto != null)
                .Cast<ContainerDto>()
                .GroupBy(containerDto => containerDto.ProjectName)
                .Select(group =>
                {
                    bool success = dbContainers.TryGetValue(group.Key, out var output);
                    var proj = output!;

                    return new ProjectDto
                    {
                        Id = success ? proj.Id : null,
                        ProjectName = success ? proj.ProjectName : group.Key,
                        ProjectDescription = success ? (proj.Description ?? "Not Managed By DockiUp") : "Not Managed By DockiUp",
                        ManagedByDockiUp = success,
                        DockerProjectName = group.Key,
                        Containers = group.ToArray(),
                        ProjectPath = success ? proj.ProjectPath : null,
                        UpdateMethod = success ? proj.ProjectUpdateMethod.ToString() : null
                    };
                }).ToArray();
        }

        public async Task<ProjectDto?> GetProjectByDockerNameAsync(string dockerProjectName)
        {
            var all = await GetProjectsAsync();
            return all.SingleOrDefault(p => string.Equals(p.DockerProjectName, dockerProjectName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<ContainerDto?> InspectContainerAsync(string containerId, CancellationToken cancellationToken = default)
        {
            var list = await _dockiUpDockerClient.DockerClient.Containers
                .ListContainersAsync(new ContainersListParameters { All = true }, cancellationToken);
            var container = list.SingleOrDefault(c => c.ID == containerId || c.ID.StartsWith(containerId, StringComparison.Ordinal));
            if (container == null)
                return null;
            container.Labels.TryGetValue("com.docker.compose.project", out string? projectName);
            container.Labels.TryGetValue("com.docker.compose.service", out string? serviceName);
            return new ContainerDto
            {
                Id = container.ID,
                Name = container.Names.SingleOrDefault()?.TrimStart('/') ?? string.Empty,
                Status = container.Status,
                State = container.State.ToEnum(),
                ProjectName = projectName ?? string.Empty,
                ServiceName = serviceName ?? string.Empty
            };
        }

        public async Task StartProjectAsync(string folderPath)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"compose -f {ComposeFileName} up -d",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = folderPath
                }
            };

            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                Console.WriteLine($"Start successfully");
            }
            else
            {
                throw new ArgumentException($"Docker Compose failed", error);
            }
        }

        public async Task RestartContainerAsync(string containerId)
        {
            await _dockiUpDockerClient.DockerClient.Containers.RestartContainerAsync(containerId, new ContainerRestartParameters { WaitBeforeKillSeconds = 10 });
        }

        public async Task StartContainerAsync(string containerId)
        {
            await _dockiUpDockerClient.DockerClient.Containers.StartContainerAsync(containerId, new ContainerStartParameters());
        }

        public async Task StopContainerAsync(string containerId)
        {
            await _dockiUpDockerClient.DockerClient.Containers.StopContainerAsync(containerId, new ContainerStopParameters());
        }

        public async Task StopProjectAsync(string folderPath)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"compose -f {ComposeFileName} down",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = folderPath
                }
            };

            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                Console.WriteLine($"Stop successfully");
            }
            else
            {
                Console.WriteLine($"Docker Compose failed");
                Console.WriteLine(error);
            }
        }

        public async Task RestartProjectAsync(string folderPath)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"compose -f {ComposeFileName} restart",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = folderPath
                }
            };

            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                Console.WriteLine($"Restart successfully");
            }
            else
            {
                Console.WriteLine($"Docker Compose failed");
                Console.WriteLine(error);
            }
        }

        public async Task<string> GetContainerLogsAsync(string containerId, int? tail = null, CancellationToken cancellationToken = default)
        {
            var parameters = new ContainerLogsParameters
            {
                ShowStdout = true,
                ShowStderr = true,
                Timestamps = false,
                Tail = tail?.ToString() ?? "100"
            };

#pragma warning disable CS0618 // Type or member is obsolete - we decode the multiplexed stream ourselves
            using var stream = await _dockiUpDockerClient.DockerClient.Containers.GetContainerLogsAsync(containerId, parameters, cancellationToken);
#pragma warning restore CS0618
            return DecodeDockerMultiplexedStream(stream);
        }

        private static string DecodeDockerMultiplexedStream(Stream stream)
        {
            var sb = new StringBuilder();
            var header = new byte[8];
            var buffer = new byte[4096];

            while (true)
            {
                int headerRead = stream.Read(header, 0, 8);
                if (headerRead < 8) break;

                // Docker stream format: [0] stream type (0=stdin, 1=stdout, 2=stderr), [1-3] padding, [4-7] size (big-endian uint32)
                int payloadSize = (header[4] << 24) | (header[5] << 16) | (header[6] << 8) | header[7];
                if (payloadSize <= 0) continue;

                int remaining = payloadSize;
                while (remaining > 0)
                {
                    int toRead = Math.Min(remaining, buffer.Length);
                    int read = stream.Read(buffer, 0, toRead);
                    if (read <= 0) break;
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, read));
                    remaining -= read;
                }
            }

            return sb.ToString();
        }
    }
}
