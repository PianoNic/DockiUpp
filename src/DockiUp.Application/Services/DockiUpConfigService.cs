using DockiUp.Application.Enums;
using DockiUp.Application.Models;
using Microsoft.Extensions.Options;

namespace DockiUp.Application.Services
{
    public class DockiUpConfigService : IDockiUpConfigService
    {
        private readonly SystemPaths _systemPaths;

        public DockiUpConfigService(IOptions<SystemPaths> systemPaths)
        {
            _systemPaths = systemPaths.Value;
        }

        public async Task<DockiupInfo> ScanForDockiUpFile(Guid containerId)
        {
            const string DockiupFileName = ".dockiup";
            string[] subDirectories = Directory.GetDirectories(_systemPaths.ProjectsPath, "*", SearchOption.TopDirectoryOnly);

            foreach (string folderPath in subDirectories)
            {
                string dockiupFilePath = Path.Combine(folderPath, DockiupFileName);
                if (!File.Exists(dockiupFilePath))
                    continue;

                string[] lines = await File.ReadAllLinesAsync(dockiupFilePath);
                DockiupInfo info = ParseDockiupFile(folderPath, lines);

                if (info.Id == containerId)
                {
                    return info;
                }
            }

            return null;
        }

        public async Task<DockiupInfo[]> ScanForDockiUpFiles()
        {
            const string DockiupFileName = ".dockiup";
            List<DockiupInfo> results = new List<DockiupInfo>();

            string[] subDirectories = Directory.GetDirectories(_systemPaths.ProjectsPath, "*", SearchOption.TopDirectoryOnly);
            foreach (string folderPath in subDirectories)
            {
                string dockiupFilePath = Path.Combine(folderPath, DockiupFileName);
                Console.WriteLine($"Found '{DockiupFileName}' in: '{folderPath}'");

                string[] lines = await File.ReadAllLinesAsync(dockiupFilePath);
                DockiupInfo info = ParseDockiupFile(folderPath, lines);
                results.Add(info);
            }

            return results.ToArray();
        }

        private static DockiupInfo ParseDockiupFile(string folderPath, string[] lines)
        {
            DockiupInfo info = new DockiupInfo { FolderPath = folderPath };

            foreach (string line in lines)
            {
                string[] parts = line.Split('=', 2);
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    if (key.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Guid.TryParse(value, out Guid guidValue))
                        {
                            info.Id = guidValue;
                        }
                    }
                    else if (key.Equals("ORIGIN", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Enum.TryParse(value, true, out ContainerOrigin originEnum))
                        {
                            info.Origin = originEnum;
                        }
                    }
                    else if (key.Equals("UPDATE_METHOD", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Enum.TryParse(value, true, out ContainerUpdateMethod updateMethodEnum))
                        {
                            info.UpdateMethod = updateMethodEnum;
                        }
                    }
                    else if (key.Equals("NAME", StringComparison.OrdinalIgnoreCase))
                    {
                        info.Name = value;
                    }
                    else if (key.Equals("LAST_UPDATED", StringComparison.OrdinalIgnoreCase))
                    {
                        if (DateTime.TryParse(value, out DateTime lastUpdated))
                        {
                            info.LastUpdated = lastUpdated;
                        }
                    }
                }
            }

            return info;
        }
    }
}
