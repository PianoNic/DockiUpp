using DockiUp.Application.Enums;

namespace DockiUp.Application.Dtos
{
    public class SetupProjectDto
    {
        public required string ProjectName { get; set; }
        public required ContainerOriginType ContainerOrigin { get; set; }
        public required ContainerUpdateMethod UpdateMethod { get; set; }
        public string? Compose { get; set; }
        public string? GitUrl { get; set; }
        public string? Path { get; set; }
    }
}
