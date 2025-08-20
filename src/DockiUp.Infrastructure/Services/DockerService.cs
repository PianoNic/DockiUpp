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
        private const string ComposeFileName = "dockiup_compose.yml";
        public DockerService(IDockiUpDockerClient dockiUpDockerClient)
        {
            _dockiUpDockerClient = dockiUpDockerClient;
        }

        public async Task<ProjectDto[]> GetProjectsAsync()
        {
            var containers = await _dockiUpDockerClient.DockerClient.Containers
                .ListContainersAsync(new ContainersListParameters { All = true });

            return containers
                .Select(container =>
                    {
                        container.Labels.TryGetValue("com.docker.compose.project", out string projectName);
                        container.Labels.TryGetValue("com.docker.compose.service", out string serviceName);

                        if (string.IsNullOrEmpty(projectName))
                            return null;

                        return new ContainerDto
                        {
                            Id = container.ID,
                            Name = container.Names.SingleOrDefault().TrimStart('/') ?? string.Empty,
                            Status = container.Status,
                            State = container.State.ToEnum(),
                            ProjectName = projectName,
                            ServiceName = serviceName
                        };
                    }
                )
                .Where(dto => dto != null)
                .GroupBy(containerDto => containerDto.ProjectName)
                .Select(group =>
                    new ProjectDto
                    {
                        Name = group.Key,
                        Containers = group.ToArray()
                    }
                )
                .ToArray();
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
                Console.WriteLine($"Docker Compose failed");
                Console.WriteLine(error);
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
                    Arguments = $"compose down",
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
                    Arguments = $"compose restart",
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
    }
}
