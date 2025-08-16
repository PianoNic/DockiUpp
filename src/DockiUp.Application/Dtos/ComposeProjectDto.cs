namespace DockiUp.Application.Dtos
{
    public class ComposeProjectDto
    {
        public required string Name { get; set; }
        public required ContainerDto[] Containers { get; set; }
    }
}