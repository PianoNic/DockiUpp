namespace DockiUp.Application.Interfaces
{
    public interface IDockiUpProjectConfigurationService
    {
        Task WriteComposeFileAsync(string projectPath, string composeContent);
    }
}
