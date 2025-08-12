using DockiUp.Application.Dtos;
using DockiUp.Application.Enums;
using DockiUp.Application.Models;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text;

namespace DockiUp.Application.Services
{
    public class DockerService : IDockerService
    {
        private readonly SystemPaths _systemPaths;
        private readonly IDockiUpConfigService _configService;
        public DockerService(IOptions<SystemPaths> systemPaths, IDockiUpConfigService configService)
        {
            _systemPaths = systemPaths.Value;
            _configService = configService;
        }

        public async Task<Guid> SetupDirectory(SetupContainerDto setupContainerDto)
        {
            string containerFolderPath = Path.Combine(_systemPaths.ProjectsPath, setupContainerDto.ContainerName);
            Directory.CreateDirectory(containerFolderPath);
            var dockiUpFileManager = new DockiUpFileManager(containerFolderPath);

            Guid containerId = Guid.Empty;

            if (setupContainerDto.ContainerOrigin == ContainerOrigin.Compose && setupContainerDto.Compose != null)
            {
                containerId = await dockiUpFileManager.GenerateDockiUpFile(setupContainerDto);
                await dockiUpFileManager.CreateComposeFile(setupContainerDto.Compose);
            }

            return containerId;
        }

        public Task PullAsync(Guid containerId)
        {
            throw new NotImplementedException();
        }

        public async Task StartAsync(Guid containerId)
        {
            var config = await _configService.ScanForDockiUpFile(containerId);

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"compose up -d",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = config.FolderPath
                }
            };

            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                Console.WriteLine($"Docker Compose executed successfully");
            }
            else
            {
                Console.WriteLine($"Docker Compose failed");
                Console.WriteLine(error);
            }
        }

        public Task StopAsync(Guid containerId)
        {
            throw new NotImplementedException();
        }

        private class DockiUpFileManager
        {
            private const string DockiUpFileName = ".dockiup";
            private readonly string ProjectPath;
            public DockiUpFileManager(string projectPath)
            {
                ProjectPath = projectPath;
            }

            public async Task<Guid> GenerateDockiUpFile(SetupContainerDto setupContainerDto)
            {
                Guid containerId = Guid.NewGuid();
                StringBuilder dockiupContent = new StringBuilder();
                dockiupContent.AppendLine($"ID={containerId}");
                dockiupContent.AppendLine($"ORIGIN={setupContainerDto.ContainerOrigin.ToString().ToUpperInvariant()}");
                dockiupContent.AppendLine($"UPDATE_METHOD={setupContainerDto.UpdateMethod.ToString().ToUpperInvariant()}");
                dockiupContent.AppendLine($"NAME={setupContainerDto.ContainerName}");
                dockiupContent.AppendLine($"LAST_UPDATED={DateTime.Now}");

                string filePath = Path.Combine(ProjectPath, DockiUpFileName);
                await File.WriteAllTextAsync(filePath, dockiupContent.ToString(), Encoding.UTF8);
                return containerId;
            }

            public async Task CreateComposeFile(string compose)
            {
                string filePath = Path.Combine(ProjectPath, "compose.yml");
                await File.WriteAllTextAsync(filePath, compose, Encoding.UTF8);
            }
        }
    }
}
