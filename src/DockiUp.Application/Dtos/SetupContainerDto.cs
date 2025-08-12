using DockiUp.Application.Enums;

namespace DockiUp.Application.Dtos
{
    public class SetupContainerDto
    {
        public required string ContainerName { get; set; }
        public required ContainerOrigin ContainerOrigin { get; set; }
        public required ContainerUpdateMethod UpdateMethod { get; set; }
        public string? Compose { get; set; }
        public string? GitUrl { get; set; }
        public string? Path { get; set; }
    }
}
