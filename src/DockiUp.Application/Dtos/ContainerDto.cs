using DockiUp.Application.Enums;

namespace DockiUp.Application.Dtos
{
    public class ContainerDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Status { get; set; }
        public required UpdateMethodType State { get; set; }
        public required string ServiceName { get; set; } // Compose Name
        public required string ProjectName { get; set; }
    }
}
