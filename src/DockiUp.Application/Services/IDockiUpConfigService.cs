using DockiUp.Application.Models;

namespace DockiUp.Application.Services
{
    public interface IDockiUpConfigService
    {
        Task<DockiupInfo[]> ScanForDockiUpFiles();
        Task<DockiupInfo> ScanForDockiUpFile(Guid ContainerId);
    }
}
