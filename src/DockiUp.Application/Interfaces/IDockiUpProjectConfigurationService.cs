namespace DockiUp.Application.Interfaces
{
    public interface IDockiUpProjectConfigurationService
    {
        Task<string> WriteComposeFileAsync(string projectPath, string composeContent);
        Task CloneRepositoryAsync(string projectPath, string gitUrl);

        /// <summary>Fetch + fast-forward the git repo checked out at <paramref name="projectPath"/>.
        /// Path-based (not id-based) so it runs identically on the control plane or a node, neither of
        /// which needs a database to pull.</summary>
        Task UpdateRepositoryAsync(string projectPath);
    }
}
