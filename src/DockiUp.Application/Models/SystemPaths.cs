namespace DockiUp.Application.Models
{
    /// <summary>
    /// System paths and Docker connection (Komodo-style: single host or socket path).
    /// </summary>
    public class SystemPaths
    {
        /// <summary>Base directory for project folders (e.g. /var/projects).</summary>
        public required string ProjectsPath { get; set; }

        /// <summary>Docker socket path (e.g. unix:///var/run/docker.sock or npipe://./pipe/docker_engine). If empty, default is used. Binds from SystemPaths__DockerSocket.</summary>
        public string? DockerSocket { get; set; }
    }
}
