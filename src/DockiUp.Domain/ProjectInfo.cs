using DockiUp.Domain.Enums;

namespace DockiUp.Domain
{
    public class ProjectInfo : EntityBase
    {
        public required string ProjectName { get; set; }
        public required string DockerProjectName { get; set; }
        public string? Description { get; set; }

        public required ProjectOriginType ProjectOrigin { get; set; }
        public string? GitUrl { get; set; }
        public required string ProjectPath { get; set; }
        public required string ComposePath { get; set; }

        public required ProjectUpdateMethod ProjectUpdateMethod { get; set; }
        public string? WebhookUrl { get; set; }
        public int? PeriodicIntervalInMinutes { get; set; }
        /// <summary>Last time periodic update ran (Komodo-style polling).</summary>
        public DateTime? LastPeriodicUpdateAt { get; set; }
    }
}
