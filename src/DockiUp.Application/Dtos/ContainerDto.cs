using DockiUp.Application.Enums;

namespace DockiUp.Application.Dtos
{
    public class ContainerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ContainerName { get; set; }
        public DateTime LastUpdated { get; set; }
        public int[] OpenPorts { get; set; }
        public ContainerState ContainerState { get; set; }
    }
}
