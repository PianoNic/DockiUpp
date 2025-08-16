using DockiUp.Application.Enums;

namespace DockiUp.Application.Models
{
    public class DockiUpProjectConfig
    {
        public string Name { get; set; } = string.Empty;
        public ContainerOriginType Origin { get; set; } = ContainerOriginType.Unknown;
        public UpdateMethodType UpdateMethod { get; set; } = UpdateMethodType.Unknown;
        public DateTime LastUpdated { get; set; } = DateTime.MinValue;
    }
}
