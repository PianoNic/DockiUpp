using DockiUp.Application.Enums;

namespace DockiUp.Application.Models
{
    public class DockiupInfo
    {
        public string FolderPath { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime LastUpdated { get; set; }
        public ContainerOrigin? Origin { get; set; }
        public ContainerUpdateMethod? UpdateMethod { get; set; }
    }
}
