using DockiUp.Application.Enums;
using DockiUp.Domain.Enums;

namespace DockiUp.Application.Models
{
    public class DockiUpProjectConfig
    {
        public string Name { get; set; } = string.Empty;
        public ProjectOriginType Origin { get; set; } = ProjectOriginType.Unknown;
        public UpdateMethodType UpdateMethod { get; set; } = UpdateMethodType.Unknown;
        public DateTime LastUpdated { get; set; } = DateTime.MinValue;
    }
}
