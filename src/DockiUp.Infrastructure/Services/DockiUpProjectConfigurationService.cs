using DockiUp.Application.Dtos;
using DockiUp.Application.Enums;
using DockiUp.Application.Interfaces;
using DockiUp.Application.Models;
using System.Text;

namespace DockiUp.Infrastructure.Services
{
    public class DockiUpProjectConfigurationService : IDockiUpProjectConfigurationService
    {
        private const string DockiUpFileName = ".dockiup";
        private const string ComposeFileName = "compose.yml";

        public DockiUpProjectConfigurationService()
        {
        }

        public async Task GenerateDockiUpConfigFileAsync(string projectPath, SetupProjectDto setupProjectDto)
        {
            StringBuilder dockiupContent = new StringBuilder();
            dockiupContent.AppendLine($"NAME={setupProjectDto.ProjectName}");
            dockiupContent.AppendLine($"ORIGIN={setupProjectDto.ContainerOrigin.ToString().ToUpperInvariant()}");
            dockiupContent.AppendLine($"UPDATE_METHOD={setupProjectDto.UpdateMethod.ToString().ToUpperInvariant()}");
            dockiupContent.AppendLine($"LAST_UPDATED={DateTime.Now:dd.MM.yyyy HH:mm:ss}");

            string filePath = Path.Combine(projectPath, DockiUpFileName);
            await File.WriteAllTextAsync(filePath, dockiupContent.ToString(), Encoding.UTF8);
        }

        public async Task WriteComposeFileAsync(string projectPath, string composeContent)
        {
            string filePath = Path.Combine(projectPath, ComposeFileName);
            await File.WriteAllTextAsync(filePath, composeContent, Encoding.UTF8);
        }

        public async Task<DockiUpProjectConfig> ReadDockiUpConfigFileAsync(string projectPath)
        {
            string filePath = Path.Combine(projectPath, DockiUpFileName);

            if (!File.Exists(filePath))
                throw new ArgumentException($"File {DockiUpFileName} does not exist");

            var config = new DockiUpProjectConfig();
            var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);

            foreach (var line in lines)
            {
                var parts = line.Split('=', 2);
                if (parts.Length != 2) continue;

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                switch (key)
                {
                    case "NAME":
                        config.Name = value;
                        break;
                    case "ORIGIN":
                        if (Enum.TryParse(value, true, out ContainerOriginType origin))
                        {
                            config.Origin = origin;
                        }
                        break;
                    case "UPDATE_METHOD":
                        if (Enum.TryParse(value, true, out UpdateMethodType updateMethod))
                        {
                            config.UpdateMethod = updateMethod;
                        }
                        break;
                    case "LAST_UPDATED":
                        if (DateTime.TryParse(value, out DateTime lastUpdated))
                        {
                            config.LastUpdated = lastUpdated;
                        }
                        break;
                }
            }

            return config;
        }
    }
}
