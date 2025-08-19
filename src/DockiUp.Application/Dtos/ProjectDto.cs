namespace DockiUp.Application.Dtos
{
    public class ProjectDto
    {
        public required string Name { get; set; }
        public required ContainerDto[] Containers { get; set; }
    }
}