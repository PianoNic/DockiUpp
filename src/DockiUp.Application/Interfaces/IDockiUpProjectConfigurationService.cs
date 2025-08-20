namespace DockiUp.Application.Interfaces
{
    public interface IDockiUpProjectConfigurationService
    {
        Task<string> WriteComposeFileAsync(string projectPath, string composeContent);
        Task CloneRepositoryAsync(string projectPath, string gitUrl);
        Task UpdateRepositoy(int projectId);
    }
}
