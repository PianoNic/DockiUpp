namespace DockiUp.Application.Services
{
    public class DockerServiceFake
    {

        //public async Task StartAsync(Guid containerId)
        //{
        //    var config = await _configService.ScanForDockiUpFile(containerId);

        //    using var process = new Process
        //    {
        //        StartInfo = new ProcessStartInfo
        //        {
        //            FileName = "docker",
        //            Arguments = $"compose up -d",
        //            RedirectStandardOutput = true,
        //            RedirectStandardError = true,
        //            UseShellExecute = false,
        //            CreateNoWindow = true,
        //            WorkingDirectory = config.FolderPath
        //        }
        //    };

        //    process.Start();

        //    string output = await process.StandardOutput.ReadToEndAsync();
        //    string error = await process.StandardError.ReadToEndAsync();

        //    await process.WaitForExitAsync();

        //    if (process.ExitCode == 0)
        //    {
        //        Console.WriteLine($"Docker Compose executed successfully");
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Docker Compose failed");
        //        Console.WriteLine(error);
        //    }
        //}

        //public Task StopAsync(Guid containerId)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task<string> GetContainerStateAsync(string containerName)
        //{
        //    using var process = new Process
        //    {
        //        StartInfo = new ProcessStartInfo
        //        {
        //            FileName = "docker",
        //            Arguments = "ps -a --format \"{{.Names}}\"",
        //            RedirectStandardOutput = true,
        //            RedirectStandardError = true,
        //            UseShellExecute = false,
        //            CreateNoWindow = true
        //        }
        //    };

        //    process.Start();

        //    string output = await process.StandardOutput.ReadToEndAsync();
        //    string error = await process.StandardError.ReadToEndAsync();

        //    await process.WaitForExitAsync();

        //    if (process.ExitCode != 0)
        //    {
        //        string trimmedError = error.Trim();
        //        Console.WriteLine($"Error listing Docker containers: {trimmedError}");

        //        return "Error";
        //    }

        //    var containerNames = output.Split(['\n'], StringSplitOptions.RemoveEmptyEntries)
        //                           .Select(name => name.Trim())
        //                           .ToList();

        //    string firstProjectContainerName = containerNames
        //    .First(name => name.StartsWith(containerName, StringComparison.OrdinalIgnoreCase));

        //    if (string.IsNullOrEmpty(firstProjectContainerName))
        //    {
        //        Console.WriteLine($"No containers found belonging to project '{containerName}'.");
        //        return "NoContainersFoundInProject";
        //    }

        //    Console.WriteLine($"Found container '{firstProjectContainerName}' for project '{containerName}'. Getting its state...");

        //    using var process2 = new Process
        //    {
        //        StartInfo = new ProcessStartInfo
        //        {
        //            FileName = "docker",
        //            Arguments = $"inspect -f '{{{{.State.Status}}}}' {firstProjectContainerName}",
        //            RedirectStandardOutput = true,
        //            RedirectStandardError = true,
        //            UseShellExecute = false,
        //            CreateNoWindow = true
        //        }
        //    };

        //    process2.Start();

        //    string output2 = await process2.StandardOutput.ReadToEndAsync();
        //    string error2 = await process2.StandardError.ReadToEndAsync();
        //    await process2.WaitForExitAsync();
        //    if (process2.ExitCode == 0)
        //    {
        //        return output2.Replace("'", "");
        //    }
        //    else
        //    {
        //        string trimmedError = error2.Trim();
        //        Console.WriteLine($"Error getting state for container '{containerName}': {trimmedError}");
        //        if (trimmedError.Contains("error during connect") ||
        //            trimmedError.Contains("Das System kann die angegebene Datei nicht finden") ||
        //            trimmedError.Contains("Cannot connect to the Docker daemon"))
        //        {
        //            Console.WriteLine("Warning: Docker daemon appears to be unavailable or not running.");
        //            return "DockerDaemonUnavailable";
        //        }
        //        else if (trimmedError.Contains("No such object"))
        //        {
        //            return "NotFound";
        //        }
        //        else
        //        {
        //            return "Error";
        //        }
        //    }
        //}

        //private class DockiUpFileManager
        //{
        //    private const string DockiUpFileName = ".dockiup";
        //    private readonly string ProjectPath;
        //    public DockiUpFileManager(string projectPath)
        //    {
        //        ProjectPath = projectPath;
        //    }

        //    public async Task<Guid> GenerateDockiUpFile(SetupProjectDto setupContainerDto)
        //    {
        //        Guid containerId = Guid.NewGuid();
        //        StringBuilder dockiupContent = new StringBuilder();
        //        dockiupContent.AppendLine($"ID={containerId}");
        //        dockiupContent.AppendLine($"ORIGIN={setupContainerDto.ContainerOrigin.ToString().ToUpperInvariant()}");
        //        dockiupContent.AppendLine($"UPDATE_METHOD={setupContainerDto.UpdateMethod.ToString().ToUpperInvariant()}");
        //        dockiupContent.AppendLine($"NAME={setupContainerDto.ContainerName}");
        //        dockiupContent.AppendLine($"LAST_UPDATED={DateTime.Now}");

        //        string filePath = Path.Combine(ProjectPath, DockiUpFileName);
        //        await File.WriteAllTextAsync(filePath, dockiupContent.ToString(), Encoding.UTF8);
        //        return containerId;
        //    }

        //    public async Task CreateComposeFile(string compose)
        //    {
        //        string filePath = Path.Combine(ProjectPath, "compose.yml");
        //        await File.WriteAllTextAsync(filePath, compose, Encoding.UTF8);
        //    }
        //}
    }
}
