using DockiUp.Domain.Enums;

namespace DockiUp.Application.Dtos
{
    public class SetupProjectDto
    {
        public required string ProjectName { get; set; }
        public string? Description { get; set; }

        public required ProjectOriginType ProjectOrigin { get; set; }
        public string? GitUrl { get; set; }
        public string? Compose { get; set; }
        public string? Path { get; set; }

        /// <summary>Optional target node. When set, the project is deployed to (and managed on) that
        /// node over SignalR; when null it runs on the local control-plane host.</summary>
        public Guid? NodeId { get; set; }

        public required ProjectUpdateMethod ProjectUpdateMethod { get; set; }
        public string? WebhookUrl { get; set; }
        public int? PeriodicIntervalInMinutes { get; set; }
    }
}
