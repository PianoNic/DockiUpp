namespace DockiUp.Application.Dtos
{
    /// <summary>What a node returns after deploying a project locally: the on-disk paths it used
    /// (computed from the node's own SystemPaths). The control plane persists these on the project
    /// row so later lifecycle calls send the node the path it actually wrote to.</summary>
    public sealed record NodeDeployResultDto(string ProjectPath, string ComposePath);
}
