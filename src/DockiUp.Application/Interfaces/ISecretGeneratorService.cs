namespace DockiUp.Application.Interfaces
{
    /// <summary>Generates high-entropy random secrets.</summary>
    public interface ISecretGeneratorService
    {
        string Generate();
    }
}
