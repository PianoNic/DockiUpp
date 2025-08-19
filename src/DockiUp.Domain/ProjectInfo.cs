using DockiUp.Domain.Enums;

namespace DockiUp.Domain
{
    public class ProjectInfo
    {
        public int Id { get; set; }
        public required string ProjectName { get; set; }
        public string? Description { get; set; }

        public required ProjectOriginType ProjectOrigin { get; set; }
        public string? GitUrl { get; set; }
        public string? Compose { get; set; }
        public required string Path { get; set; }
        public required string ComposePath { get; set; }

        public required ProjectUpdateMethod ProjectUpdateMethod { get; set; }
        public string? WebhookUrl { get; set; }
        public int? PeriodicIntervalInMinutes { get; set; }

        public required DateTime LastUpdated { get; set; }
    }
}
