using DockiUp.Application.Dtos;
using DockiUp.Application.Models;

namespace DockiUp.Application.Interfaces
{
    public interface IDockiUpProjectConfigurationService
    {
        Task GenerateDockiUpConfigFileAsync(string projectPath, SetupProjectDto setupProjectDto);
        Task WriteComposeFileAsync(string projectPath, string composeContent);
        Task<DockiUpProjectConfig> ReadDockiUpConfigFileAsync(string projectPath);
    }
}
