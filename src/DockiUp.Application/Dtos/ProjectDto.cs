namespace DockiUp.Application.Dtos
{
    public class ProjectDto
    {
        public required string ProjectName { get; set; }
        public required string DockerProjectName { get; set; }
        public required string ProjectDescription { get; set; }
        public required bool ManagedByDockiUp { get; set; }
        public required ContainerDto[] Containers { get; set; }
    }
}